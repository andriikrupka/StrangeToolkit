namespace StrangeToolkit.Storage
{
    using StrangeToolkit.Serializer;
    using StrangeToolkit.Synchonization;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Storage;

    public class StorageProvider
    {
        private object settingsLockerLock = new object();
        private object filesLockerLock = new object();
        private Dictionary<string, object> settingsLockers = new Dictionary<string, object>();
        private Dictionary<string, AsyncLock> fileLockers = new Dictionary<string, AsyncLock>();
        private StorageFolder localStateFolder;
        private Serializer storageSerializer;

        private readonly static Lazy<StorageProvider> instance = new Lazy<StorageProvider>(() => new StorageProvider(), true);

        public static StorageProvider Instance
        {
            get
            {
                return instance.Value;
            }
        }

        public StorageProvider()
        {
            this.localStateFolder = ApplicationData.Current.LocalFolder;
            this.storageSerializer = new JsonSerializer();
        }

        /// <summary>
        /// Returns symbol length for local folder
        /// </summary>
        public int LocalFolderPathLength
        {
            get
            {
                var value = 0;
                if (this.localStateFolder != null)
                {
                    value = this.localStateFolder.Path.Length;
                }

                return value;
            }
        }
        
        #region Read/Write/Delete Settings

        public void WriteToSettings<T>(string settingsKey, T value)
        {
            var settings = ApplicationData.Current.LocalSettings;
            var fileLock = this.GetSettingsLock(settingsKey);
            lock (fileLock)
            {
                if (!settings.Values.Keys.Contains(settingsKey))
                {
                    settings.Values.Add(new KeyValuePair<string, object>(settingsKey, value));
                }
                else
                {
                    settings.Values[settingsKey] = value;
                }
            }
        }

        public T ReadFromSettings<T>(string settingsKey)
        {
            var result = default(T);
            var settings = ApplicationData.Current.LocalSettings;
            var fileLock = this.GetSettingsLock(settingsKey);
            object value;
            lock (fileLock)
            {
                settings.Values.TryGetValue(settingsKey, out value);
            }

            if (value != null)
            {
                result = (T)value;
            }

            return result;
        }

        public void DeleteFromSettings(string settingsKey)
        {
            var settings = ApplicationData.Current.LocalSettings;
            var fileLock = this.GetSettingsLock(settingsKey);
            lock (fileLock)
            {
                if (settings.Values.Keys.Contains(settingsKey))
                {
                    settings.Values.Remove(settingsKey);
                }
            }
        }
        #endregion

        # region Read/Write/Delete File
        public async Task WriteToFileAsync<T>(string key, T value)
        {
            await this.WriteToFileAsync<T>(key, value, this.storageSerializer);
        }

        public async Task WriteToFileAsync<T>(string key, T value, Serializer serializer)
        {
            var fileLock = this.GetFileLock(key);

            using (await fileLock.LockAsync())
            {
                var filePath = FilePath.CreateFilePath(key);
                var folder = await this.OpenFolderLocation(filePath.Folders);
                var storageFile = await folder.CreateFileAsync(filePath.FileName, CreationCollisionOption.ReplaceExisting);
                using (var randomStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var stream = randomStream.AsStream())
                    {
                        serializer.Serialize<T>(value, stream);
                    }
                }
            }
        }

        public async Task<T> ReadFromFileAsync<T>(string key)
        {
            return await this.ReadFromFileAsync<T>(key, this.storageSerializer);
        }

        public async Task<T> ReadFromFileAsync<T>(string key, Serializer serializer)
        {
            var returnObj = default(T);

            var fileLock = this.GetFileLock(key);

            using (await fileLock.LockAsync())
            {
                var filePath = FilePath.CreateFilePath(key);
                var folder = await this.OpenFolderLocation(filePath.Folders);


                var storageFile = await folder.CreateFileAsync(filePath.FileName, CreationCollisionOption.OpenIfExists);
                if (storageFile != null)
                {
                    using (var randomStream = await storageFile.OpenAsync(FileAccessMode.Read))
                    {
                        using (var stream = randomStream.AsStream())
                        {
                            returnObj = serializer.Deserialize<T>(stream);
                        }
                    }
                }

            }

            return returnObj;
        }

        public async Task DeleteFileAsync(string key)
        {
            var fileLock = this.GetFileLock(key);
            using (await fileLock.LockAsync())
            {
                var filePath = FilePath.CreateFilePath(key);
                var folder = await this.OpenFolderLocation(filePath.Folders);
                var storageFile = await folder.CreateFileAsync(filePath.FileName, CreationCollisionOption.OpenIfExists);
                if (storageFile != null)
                {
                    await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
        }

        #endregion

        private async Task<StorageFolder> OpenFolderLocation(IEnumerable<string> nestedFolders)
        {
            var fileFolder = this.localStateFolder;

            foreach (var folderName in nestedFolders)
            {
                fileFolder = await fileFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            }

            return fileFolder;
        }

        #region File / Settings Locker
        private object GetSettingsLock(string key)
        {
            lock (this.settingsLockerLock)
            {
                if (!this.settingsLockers.Keys.Contains(key))
                {
                    this.settingsLockers.Add(key, new object());
                }

                return this.settingsLockers[key];
            }
        }

        private AsyncLock GetFileLock(string key)
        {
            lock (this.filesLockerLock)
            {
                if (!this.fileLockers.Keys.Contains(key))
                {
                    this.fileLockers.Add(key, new AsyncLock());
                }

                return this.fileLockers[key];
            }
        }
        #endregion

        public Task<StorageFolder> CreateFolderAsync(string folderName)
        {
            var nestedFolders = FilePath.CreateFolders(folderName);
            return this.OpenFolderLocation(nestedFolders);
        }
    }
}
