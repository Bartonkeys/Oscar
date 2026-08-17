using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Oscar.Mrit.Features.FelixMrit.Mapping
{
    public static class ParseHelpers
    {
        public static string SimplfyText(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var updated = Regex.Replace(text.ToUpper().ToLower().Trim(), "&", "and");
            updated = Regex.Replace(updated, "\\.|,|!|\"|\\(|\\)|\\*|'|;|:|\\?|-|_|\\^|£|\\$|\\s+|•|`", "");
            updated = updated.Replace("ẞ", "ss");
            updated = updated.Replace("ß", "ss");
            updated = updated.Replace("æ", "ae");
            updated = updated.Replace("ø", "o");
            updated = updated.Replace("å", "a");
            updated = updated.Replace("œ", "oe");
            updated = updated.Replace("þ", "th");
            updated = updated.Replace("·", "");
            updated = updated.Replace("ð", "d");
            updated = updated.Replace("«", "");
            updated = updated.Replace("»", "");
            updated = updated.Replace(((char)8206).ToString(), string.Empty);
            updated = updated.Replace(((char)65279).ToString(), string.Empty);

            updated = updated.Normalize(NormalizationForm.FormD);
            var chars = updated.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            updated = new string(chars).Normalize(NormalizationForm.FormC);
            updated = updated.Normalize(NormalizationForm.FormKD);
            chars = updated.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormKC);
        }

        public static HashSet<string> Titles = new HashSet<string> { "mr", "ms", "mrs", "miss", "dr", "sir", "dame", "lady", "lord" };

        public static string SimplifyPersonName(this string wholeName)
        {
            if (string.IsNullOrWhiteSpace(wholeName))
            {
                return string.Empty;
            }

            var names = wholeName.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Regex.Replace(x.Trim(), @"\.|\,", "").Replace(((char)8206).ToString(), string.Empty).Replace(((char)65279).ToString(), string.Empty));

            var title = string.Empty;
            if (Titles.Contains(names.First().ToLower()) && names.Count() != 1)
            {
                title = names.First();
                names = names.Where((x, i) => i > 0).ToArray();
            }

            return string.Join(' ', names).SimplfyText();
        }

        public static bool IsAllCaps(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return false;
            }

            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(word[i]) && !char.IsUpper(word[i]))
                    return false;
            }
            return true;
        }

        public static string RemoveAllCaps(string word)
        {
            if (string.IsNullOrWhiteSpace(word) || !IsAllCaps(word))
            {
                return word;
            }

            var words = word.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();

            var newWords = new List<string>();
            foreach (var wrd in words)
            {
                newWords.Add(wrd[0] + wrd.Substring(1, wrd.Length - 1).ToLower());
            }

            return string.Join(" ", newWords);
        }

    }
}
