using Ganss.Xss;

namespace Server.Security
{
    public static class MessageSanitizer
    {
        private static readonly HtmlSanitizer _sanitizer = new HtmlSanitizer();

        static MessageSanitizer()
        {
            // Allow basic formatting tags
            _sanitizer.AllowedTags.Add("b");
            _sanitizer.AllowedTags.Add("i");
            _sanitizer.AllowedTags.Add("u");
            _sanitizer.AllowedTags.Add("img");
            _sanitizer.AllowedAttributes.Add("src");
        }

        public static string Sanitize(string content)
        {
            return _sanitizer.Sanitize(content);
        }
    }
} 