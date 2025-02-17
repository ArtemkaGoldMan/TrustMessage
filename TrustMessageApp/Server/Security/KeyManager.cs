﻿using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Server.Security
{
    public class KeyManager
    {
        private readonly IConfiguration _configuration;

        public KeyManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string PublicKey, string PrivateKey) GenerateRsaKeyPair()
        {
            using var rsa = RSA.Create(2048);
            return (
                rsa.ExportSubjectPublicKeyInfoPem(),
                rsa.ExportPkcs8PrivateKeyPem()
            );
        }

        public string EncryptPrivateKey(string privateKey, string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(
                password, 
                Encoding.UTF8.GetBytes("TrustMessageApp"), 
                10000, 
                HashAlgorithmName.SHA256
            );
            using var aes = Aes.Create();
            aes.Key = deriveBytes.GetBytes(32); // 256-bit key
            aes.IV = deriveBytes.GetBytes(16);  // 128-bit IV
            
            using var encryptor = aes.CreateEncryptor();
            byte[] privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(privateKeyBytes, 0, privateKeyBytes.Length);
            
            return Convert.ToBase64String(encryptedBytes);
        }

        public string DecryptPrivateKey(string encryptedPrivateKey, string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(
                password, 
                Encoding.UTF8.GetBytes("TrustMessageApp"), 
                10000, 
                HashAlgorithmName.SHA256
            );
            using var aes = Aes.Create();
            aes.Key = deriveBytes.GetBytes(32); // 256-bit key
            aes.IV = deriveBytes.GetBytes(16);  // 128-bit IV
            
            using var decryptor = aes.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64String(encryptedPrivateKey);
            byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public string SignData(string data, string privateKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);
            
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            
            return Convert.ToBase64String(signatureBytes);
        }

        public bool VerifyData(string data, string signature, string publicKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);
            
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = Convert.FromBase64String(signature);
            
            return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
