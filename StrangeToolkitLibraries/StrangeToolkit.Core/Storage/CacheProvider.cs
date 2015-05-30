namespace StrangeToolkit.Storage
{
    using StrangeToolkit.Cryptography;
    using StrangeToolkit.Synchonization;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    public class CacheProvider
    {
        private const string CachedKey = "Cache";

        private const int MaxFileLength = 250;

        private const string CachedFilePathPattern = "/" + CachedKey + "/{0}";

        private const string LifeTimeCachedKey = "CacheLifeTime";

        private readonly object cacheLifetimeLock = new object();

        private readonly AsyncLock saveCacheMapLocker = new AsyncLock();

        private static readonly CacheProvider instance = new CacheProvider();

        private Dictionary<string, DateTime> lifeTimeCachedFiles;

        public TimeSpan MaxLifePeriod { get; set; }

        private Task initializeTask;

        public static CacheProvider Instance
        {
            get { return instance; }
        }

        public CacheProvider()
        {
            this.MaxLifePeriod = TimeSpan.FromDays(30);
            this.LifeDuration = TimeSpan.FromMinutes(30);
            this.InitializeAsync();
        }

        public TimeSpan LifeDuration { get; set; }

        private Task InitializeAsync()
        {
            if (this.initializeTask == null)
            {
                this.initializeTask = Task.Run(async () =>
                {
                    await StorageProvider.Instance.CreateFolderAsync(CachedKey);
                    if (this.lifeTimeCachedFiles == null)
                    {
                        this.lifeTimeCachedFiles = await
                            StorageProvider.Instance.ReadFromFileAsync<Dictionary<string, DateTime>>(LifeTimeCachedKey)
                            ?? new Dictionary<string, DateTime>();
                    }
                });
            }

            return initializeTask;
        }

        public bool IsExpired(string key)
        {
            key = this.CreateFilePath(key);
            DateTime addedTime;
            this.lifeTimeCachedFiles.TryGetValue(key, out addedTime);
            var durationDifference = addedTime - DateTime.UtcNow;
            return durationDifference > this.LifeDuration;
        }

        public async Task<T> ReadFile<T>(string key)
        {
            await initializeTask;
            key = this.CreateFilePath(key);
            var result = await StorageProvider.Instance.ReadFromFileAsync<T>(key).ConfigureAwait(false);

            if (Object.Equals(result, default(T)))
            {
                result = (T)Activator.CreateInstance(typeof(T));
            }

            return result;
        }

        public async Task WriteFile<T>(string key, T value)
        {
            await this.initializeTask;
            key = this.CreateFilePath(key);
            await StorageProvider.Instance.WriteToFileAsync(key, value);
            this.AddFileToLifeTimeMap(key);
        }

        private void AddFileToLifeTimeMap(string key)
        {
            lock (this.cacheLifetimeLock)
            {
                this.lifeTimeCachedFiles[key] = DateTime.UtcNow;
            }

            this.SaveCacheLifeTimeMap();
        }

        private async void SaveCacheLifeTimeMap()
        {
            Dictionary<string, DateTime> savedData = null;

            lock (cacheLifetimeLock)
            {
                savedData = new Dictionary<string, DateTime>(this.lifeTimeCachedFiles);
            }

            using (await this.saveCacheMapLocker.LockAsync())
            {
                await StorageProvider.Instance.WriteToFileAsync(LifeTimeCachedKey, savedData);
            }
        }

        private string CreateFilePath(string key)
        {
            var length = StorageProvider.Instance.LocalFolderPathLength;
            var localPath = string.Format(CultureInfo.InvariantCulture, CachedFilePathPattern, key);
            if (length + localPath.Length > MaxFileLength)
            {
                key = Md5Calculator.ComputeMd5(key);
            }

            return string.Format(CultureInfo.InvariantCulture, CachedFilePathPattern, key);
        }

        public Task DeleteOldFiles()
        {
            return Task.Run(async () =>
            {
                await this.initializeTask;
                var nowTime = DateTime.Now;
                var removedFiles =
                    this.lifeTimeCachedFiles.Where(l => (nowTime - l.Value) > this.MaxLifePeriod).Select(l => l.Key).ToList();

                var deletedTasks = removedFiles.Select(file => StorageProvider.Instance.DeleteFileAsync(file));
                lock (this.cacheLifetimeLock)
                {
                    foreach (var item in removedFiles)
                    {
                        this.lifeTimeCachedFiles.Remove(item);
                    }
                }

                this.SaveCacheLifeTimeMap();
                await Task.WhenAll(deletedTasks);
            });
        }
    }
}
