using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CsFromFolder
{
    static class TemplateFormatter
    {
        const string FOR_EACH_FILE_REGION_NAME = "FOR EACH FILE";

        const string FILE_NAME_AS_VALID_NAME = "";

        private static readonly IDictionary<string, Func<string, string>> fileNameReplaces;

        static TemplateFormatter()
        {
            fileNameReplaces = new Dictionary<string, Func<string, string>>()
            {
                { "FILE_PATH_PASCAL_CASE", FilePathFormatter.ToPascalCaseWithoutExtension },
                { "FILE_PATH_CAMEL_CASE", FilePathFormatter.ToCamelCaseWithoutExtension}
            };
        }

        public static string FillInTemplate(string template, TemplateData data)
        {
            Regex forEachRegionRegex = GetForEachRegionRegex();
            var filledIn = forEachRegionRegex
                .Replace(template, (m) =>
                {
                    const int foreachContentRegexGroup = 1;
                    string foreachContent = m.Groups[foreachContentRegexGroup].Value;
                    return ForEachEvaluator(foreachContent, data);
                });

            return filledIn;
        }

        private static Regex GetForEachRegionRegex()
        {
            string forEachPattern = GetForEachRegionPattern();
            var forEachRegex = new Regex(forEachPattern, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            return forEachRegex;
        }

        private static string GetForEachRegionPattern()
        {
            string foreachRegionBegin = Regex.Escape("#region " + FOR_EACH_FILE_REGION_NAME);
            string foreachRegionContent = @" .+? ^ (.*?) ^ \s+";
            string foreachRegionEnd = Regex.Escape("#endregion");

            return foreachRegionBegin + foreachRegionContent + foreachRegionEnd;
        }

        private static string ForEachEvaluator(string forEachTemplate, TemplateData data)
        {
            var filledInForeachRegion = new StringBuilder();

            foreach (var fileName in data.RelativeFilesPaths)
            {
                StringBuilder filledInIteration = ReplaceAllKeywordsWithTransformedFileNames(forEachTemplate, fileName);

                filledInForeachRegion.Append(filledInIteration);
            }

            return filledInForeachRegion.ToString();
        }

        private static StringBuilder ReplaceAllKeywordsWithTransformedFileNames(string template, string source)
        {
            StringBuilder processedText = new StringBuilder(template);

            foreach (string keywordToReplace in fileNameReplaces.Keys)
            {
                Func<string, string> fileNameTransformation = fileNameReplaces[keywordToReplace];
                string transfromedFileName = fileNameTransformation(source);

                processedText.Replace(keywordToReplace, transfromedFileName);
            }

            return processedText;
        }
    }
}
