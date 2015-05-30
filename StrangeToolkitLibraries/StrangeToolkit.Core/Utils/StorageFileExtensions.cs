namespace StrangeToolkit.Utils
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.Storage;

    public static class StorageFileExtensions
    {
        public static async Task<bool> IsFileExistAsync(this StorageFolder folder, string fileName)
        {
            var isExist = false;

            try
            {
                await folder.GetFileAsync(fileName);
                isExist = true;
            }
            catch (FileNotFoundException)
            {
                
            }

            return isExist;
        }
    }
}
