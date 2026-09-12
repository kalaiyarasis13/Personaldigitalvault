using PersonalDigitalVaultBackend.Services.Interface;

namespace PersonalDigitalVaultBackend.Services.Implementations
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _rootPath;

        public FileStorageService(IConfiguration configuration, IWebHostEnvironment env)
        {
            var configuredPath = configuration["FileStorage:RootPath"] ?? "App_Data/VaultFiles";
            _rootPath = Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(env.ContentRootPath, configuredPath);

            Directory.CreateDirectory(_rootPath);
        }

        private string UserFolder(int userId)
        {
            var path = Path.Combine(_rootPath, userId.ToString());
            Directory.CreateDirectory(path);
            return path;
        }

        public async Task<string> SaveEncryptedFileAsync(byte[] encryptedBytes, string originalFileName, int userId)
        {
            var extension = Path.GetExtension(originalFileName);
            var storedFileName = $"{Guid.NewGuid()}{extension}.enc";
            var fullPath = Path.Combine(UserFolder(userId), storedFileName);

            await File.WriteAllBytesAsync(fullPath, encryptedBytes);
            return storedFileName;
        }

        public async Task<byte[]> ReadEncryptedFileAsync(string storedFileName, int userId)
        {
            var fullPath = Path.Combine(UserFolder(userId), storedFileName);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Stored file not found.");

            return await File.ReadAllBytesAsync(fullPath);
        }

        public void DeleteFile(string storedFileName, int userId)
        {
            var fullPath = Path.Combine(UserFolder(userId), storedFileName);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
