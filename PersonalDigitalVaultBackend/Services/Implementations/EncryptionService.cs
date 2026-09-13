using PersonalDigitalVaultBackend.Services.Interface;
using System.Security.Cryptography;
using System.Text;

namespace PersonalDigitalVaultBackend.Services.Implementations
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(IConfiguration configuration)
        {
            var configuredKey = configuration["Encryption:Key"]
                ?? throw new InvalidOperationException("Encryption:Key is not configured.");

            // Key must decode to exactly 32 bytes (AES-256). Falls back to deriving
            // a 32-byte key from the configured string if it is not valid Base64/32 bytes,
            // so the app still runs with the placeholder value from appsettings.json.
            try
            {
                var decoded = Convert.FromBase64String(configuredKey);
                _key = decoded.Length == 32 ? decoded : SHA256.HashData(Encoding.UTF8.GetBytes(configuredKey));
            }
            catch (FormatException)
            {
                _key = SHA256.HashData(Encoding.UTF8.GetBytes(configuredKey));
            }
        }

        public string EncryptText(string plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = EncryptBytes(plainBytes);
            return Convert.ToBase64String(cipherBytes);
        }

        public string DecryptText(string cipherTextBase64)
        {
            var cipherBytes = Convert.FromBase64String(cipherTextBase64);
            var plainBytes = DecryptBytes(cipherBytes);
            return Encoding.UTF8.GetString(plainBytes);
        }

        public byte[] EncryptBytes(byte[] plainBytes)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var cipher = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Prefix the IV so it travels with the ciphertext - needed to decrypt later.
            var result = new byte[aes.IV.Length + cipher.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipher, 0, result, aes.IV.Length, cipher.Length);
            return result;
        }

        public byte[] DecryptBytes(byte[] cipherBytes)
        {
            using var aes = Aes.Create();
            aes.Key = _key;

            var ivLength = aes.BlockSize / 8; // 16 bytes for AES
            var iv = new byte[ivLength];
            var cipher = new byte[cipherBytes.Length - ivLength];
            Buffer.BlockCopy(cipherBytes, 0, iv, 0, ivLength);
            Buffer.BlockCopy(cipherBytes, ivLength, cipher, 0, cipher.Length);

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        }

        public string ComputeSha256(byte[] data)
        {
            var hashBytes = SHA256.HashData(data);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
