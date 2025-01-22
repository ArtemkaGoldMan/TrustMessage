using OtpNet;
namespace Server.Security
{
    public class TwoFactorAuthService
    {
        public static string GenerateSecretKey()
        {
            byte[] secretKey = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(secretKey);
        }

        public static string GetQrCodeUri(string username, string secret)
        {
            return $"otpauth://totp/MyApp:{username}?secret={secret}&issuer=MyApp";
        }

        public static bool ValidateTOTP(string secret, string code)
        {
            var keyBytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(keyBytes);
            return totp.VerifyTotp(code, out _);
        }
    }
}
