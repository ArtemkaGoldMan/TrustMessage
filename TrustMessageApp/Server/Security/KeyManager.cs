using System.Security.Cryptography;
using System.Text;

namespace Server.Security
{
    public static class KeyManager
    {
        // Generate RSA key pair
        public static (string PublicKey, string PrivateKey) GenerateRsaKeyPair()
        {
            using var rsa = RSA.Create(2048); // 2048-bit key size
            string publicKey = rsa.ExportSubjectPublicKeyInfoPem();
            string privateKey = rsa.ExportPkcs8PrivateKeyPem();

            return (publicKey, privateKey);
        }

        // Encrypt the private key using AES
        public static string EncryptPrivateKey(string privateKey, string password)
        {
            using var aes = Aes.Create();
            // Convert the hashed password string to a byte array
            aes.Key = Encoding.UTF8.GetBytes(PBKDF2Hasher.HashPassword(password)).Take(32).ToArray(); // Use first 32 bytes of hashed password as key
            aes.IV = new byte[16]; // Use a fixed or random IV (ensure it's stored securely if random)

            using var encryptor = aes.CreateEncryptor();
            byte[] privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(privateKeyBytes, 0, privateKeyBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string DecryptPrivateKey(string encryptedPrivateKey, string password)
        {
            using var aes = Aes.Create();
            // Convert the hashed password string to a byte array
            aes.Key = Encoding.UTF8.GetBytes(PBKDF2Hasher.HashPassword(password)).Take(32).ToArray(); // Use first 32 bytes of hashed password as key
            aes.IV = new byte[16]; // Use the same IV used for encryption

            using var decryptor = aes.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64String(encryptedPrivateKey);
            byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public static string SignData(string data, string username, string privateKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);

            // Combine the data and username into a single string for signing
            string combinedData = $"{data}|{username}";
            byte[] dataBytes = Encoding.UTF8.GetBytes(combinedData);
            byte[] signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signatureBytes);
        }

        public static bool VerifyData(string data, string username, string signature, string publicKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);

            // Combine the data and username into a single string for verification
            string combinedData = $"{data}|{username}";
            byte[] dataBytes = Encoding.UTF8.GetBytes(combinedData);
            byte[] signatureBytes = Convert.FromBase64String(signature);

            return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
