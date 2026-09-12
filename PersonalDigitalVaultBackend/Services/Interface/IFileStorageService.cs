namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface IFileStorageService
    {
        /// <summary>Saves already-encrypted bytes to the protected storage folder, returns the stored file name.</summary>
        Task<string> SaveEncryptedFileAsync(byte[] encryptedBytes, string originalFileName, int userId);

        Task<byte[]> ReadEncryptedFileAsync(string storedFileName, int userId);

        void DeleteFile(string storedFileName, int userId);
    }
}
