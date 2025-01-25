using System.Security.Cryptography;
using System.Text;

namespace Server.Security
{
    public static class MessageSigner
    {
        public static string SignMessage(string content, string privateKey)
        {
            using RSA rsa = RSA.Create();
            rsa.FromXmlString(privateKey);
            byte[] data = Encoding.UTF8.GetBytes(content);
            byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }

        public static bool VerifyMessage(string content, string signature, string publicKey)
        {
            using RSA rsa = RSA.Create();
            rsa.FromXmlString(publicKey);
            byte[] data = Encoding.UTF8.GetBytes(content);
            byte[] signatureBytes = Convert.FromBase64String(signature);
            return rsa.VerifyData(data, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
