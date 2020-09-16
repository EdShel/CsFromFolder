using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CsFromFolder
{
    public static class FilePathFormatter
    {
        public static string ToPascalCaseWithoutExtension(string filePath)
        {
            string[] words = SplitPathByWordsWithoutExtension(filePath);
            string pascalCased = WordsToPascalCaseString(words);

            return pascalCased;
        }

        private static string[] SplitPathByWordsWithoutExtension(string filePath)
        {
            string withoutExt = RemoveFileExtension(filePath);
            string[] words = SplitFilePathByWords(withoutExt);
            return words;
        }

        private static string RemoveFileExtension(string filePath)
        {
            return Path.ChangeExtension(filePath, null);
        }

        private static string[] SplitFilePathByWords(string filePath)
        {
            var wordPattern = @"[A-Z]?[a-z0-9]+";
            var wordSearchExpression = new Regex(wordPattern);

            var wordMatches = wordSearchExpression.Matches(filePath);
            return wordMatches.Select(m => m.Value).ToArray();
        }

        private static string WordsToPascalCaseString(string[] words)
        {
            var sentenceCased = words.Select(word => SentesceCaseString(word));
            var concattedText = String.Concat(sentenceCased);
            return concattedText;
        }

        private static string SentesceCaseString(string asciiText)
        {
            string capitalLetter = char.ToUpper(asciiText[0]).ToString();
            string lowerCasePart = asciiText.Substring(1).ToLower();
            return capitalLetter + lowerCasePart;
        }

        public static string ToCamelCaseWithoutExtension(string filePath)
        {
            string pascalCased = ToPascalCaseWithoutExtension(filePath);
            string camelCase = LowerFirstLetter(pascalCased);

            return camelCase;
        }

        private static string LowerFirstLetter(string text)
        {
            string firstLetter = char.ToLower(text[0]).ToString();
            string otherPart = text.Substring(1);

            return firstLetter + otherPart;
        }
    }
}
