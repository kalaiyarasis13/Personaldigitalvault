namespace PersonalDigitalVaultBackend.Services.Interface
{
    public interface IEncryptionService
    {
        /// <summary>Encrypts a plaintext string with AES, returns Base64(IV + CipherText).</summary>
        string EncryptText(string plainText);

        /// <summary>Reverses EncryptText.</summary>
        string DecryptText(string cipherTextBase64);

        /// <summary>Encrypts raw file bytes with AES, prefixing the IV to the returned buffer.</summary>
        byte[] EncryptBytes(byte[] plainBytes);

        /// <summary>Reverses EncryptBytes.</summary>
        byte[] DecryptBytes(byte[] cipherBytes);

        /// <summary>Computes a SHA-256 hash (hex string) of the given bytes, for integrity checking.</summary>
        string ComputeSha256(byte[] data);
    }
}
