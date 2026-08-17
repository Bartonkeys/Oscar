using System.Text;
using System.ComponentModel;

namespace Oscar.Infrastructure.Features.Common
{
    public static class StringExtensions
    {
        public static string DecodeBase64(this string base64EncodedData)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string EncodeBase64(this string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        public static string CleanseOf(this string text, char character)
        {
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                if (c != character)
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }

}
