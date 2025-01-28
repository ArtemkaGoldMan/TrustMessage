namespace Server.Security
{
    public static class PasswordStrengthChecker
    {
        public static double CalculateEntropy(string password)
        {
            if (string.IsNullOrEmpty(password))
                return 0;

            int charsetSize = 0;
            if (password.Any(char.IsLower)) charsetSize += 26; // a-z
            if (password.Any(char.IsUpper)) charsetSize += 26; // A-Z
            if (password.Any(char.IsDigit)) charsetSize += 10; // 0-9
            if (password.Any(ch => "!@#$%^&*()_+{}|:<>?".Contains(ch))) charsetSize += 20; // Special characters

            double entropy = password.Length * Math.Log2(charsetSize);
            return entropy;
        }

        public static bool IsPasswordStrong(string password)
        {
            return CalculateEntropy(password) >= 60; 
        }
    }
}
