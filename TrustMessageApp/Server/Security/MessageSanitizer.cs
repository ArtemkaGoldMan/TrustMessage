using Markdig;
using Ganss.Xss;
using System.Text.RegularExpressions;

namespace Server.Security
{
    public static class MessageSanitizer
    {
        private static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
            .DisableHtml()
            .UseAdvancedExtensions()
            .Build();

        private static readonly Regex urlRegex = new Regex(
            @"^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\- .\/?%&=]*)?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string Sanitize(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            // Convert Markdown to HTML
            var html = Markdown.ToHtml(content, pipeline);
            
            // Configure sanitizer with strict rules
            var sanitizer = new HtmlSanitizer();
            
            // Allow basic formatting tags
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedTags.Add("p");
            sanitizer.AllowedTags.Add("strong");
            sanitizer.AllowedTags.Add("em");
            sanitizer.AllowedTags.Add("ul");
            sanitizer.AllowedTags.Add("ol");
            sanitizer.AllowedTags.Add("li");
            sanitizer.AllowedTags.Add("blockquote");
            sanitizer.AllowedTags.Add("code");
            sanitizer.AllowedTags.Add("pre");
            sanitizer.AllowedTags.Add("a");
            
            // Configure allowed attributes
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedAttributes.Add("href");
            
            // Add URL validation
            sanitizer.RemovingAttribute += (s, e) => 
            {
                if (e.Attribute.Name == "href" && 
                    !string.IsNullOrEmpty(e.Attribute.Value) &&
                    !IsValidUrl(e.Attribute.Value))
                {
                    e.Cancel = true;
                    e.Attribute.Value = "#invalid-url";
                }
            };
            
            
            return sanitizer.Sanitize(html);
        }

        private static bool IsValidUrl(string url)
        {
            // Validate URL format
            if (!urlRegex.IsMatch(url))
                return false;

            // Only allow http and https protocols
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
} 