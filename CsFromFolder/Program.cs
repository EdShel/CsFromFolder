using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CsFromFolder
{
    class Program
    {
        static void Main(string[] args)
        {
            ProgramArgs options = GetProgramParameters(args);
            if (options == null)
            {
                return;
            }

            string templateFilePath = options.TemplateFilePath.FullName;
            string resourcesDirectory = options.ResourcesFolder.ToString();
            string searchPattern = options.ResourceSearchPattern;

            var filledIn = TemplateFormatter.FillInTemplate(
                template: File.ReadAllText(templateFilePath),
                data: new TemplateData(
                    files: DirectoryScanner
                        .GetDirectoryFilesFullPathsRecursively(resourcesDirectory, searchPattern)
                        .Select(fileAbs => Path.GetRelativePath(resourcesDirectory, fileAbs))));

            Console.WriteLine(filledIn);
            Console.Read();
        }

        private static ProgramArgs GetProgramParameters(string[] args)
        {
            CommandLineParser.CommandLineParser parser = new CommandLineParser.CommandLineParser();

            try
            {
                return ExtractArgumentsByFrom(parser, args);
            }
            catch (CommandLineException ex)
            {
                HandleArgumentsParsingError(parser, ex);
                throw ex;
            }
        }

        private static ProgramArgs ExtractArgumentsByFrom(CommandLineParser.CommandLineParser parser, string[] args)
        {
            var options = new ProgramArgs();
            parser.ExtractArgumentAttributes(options);
            parser.ParseCommandLine(args);

            if (!parser.ParsingSucceeded)
            {
                return null;
            }

            return options;
        }

        private static void HandleArgumentsParsingError(CommandLineParser.CommandLineParser parser, CommandLineException ex)
        {
            Console.Error.WriteLine(ex.Message);
            parser.ShowUsage();
        }
    }

    class ProgramArgs
    {
        [FileArgument('t', "template",
            Description = "template file with code to be filled in with files data",
            Optional = false, FileMustExist = true)]
        public FileInfo TemplateFilePath { set; get; }

        [DirectoryArgument('r', "resources",
            Description = "a folder with files to be inserted into the template",
            Optional = false,
            DirectoryMustExist = true)]
        public DirectoryInfo ResourcesFolder { set; get; }

        [ValueArgument(typeof(string), 's', "selector",
            Description = "selector to capture files in the resources folder",
            ForcedDefaultValue = "*")]
        public string ResourceSearchPattern { set; get; }
    }

    sealed class TemplateData
    {
        public IEnumerable<string> RelativeFilesPaths { get; private set; }

        public TemplateData(IEnumerable<string> files)
        {
            RelativeFilesPaths = files.ToArray();
        }
    }

    static class TemplateFormatter
    {
        const string FOR_EACH_FILE_REGION_NAME = "FOR EACH FILE";

        const string FILE_NAME_AS_VALID_NAME = "FILE_PATH_PASCAL_CASE";

        public static string FillInTemplate(string template, TemplateData data)
        {
            Regex forEachRegionRegex = GetForEachRegionRegex();
            var filledIn = forEachRegionRegex.Replace(template, (m) => ForEachEvaluator(m.Groups[1].Value, data));

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

            const string keywordToReplace = FILE_NAME_AS_VALID_NAME;
            foreach (var fileName in data.RelativeFilesPaths.Select(path => FilePathFormatter.ToPascalCaseWithoutExtension(path)))
            {
                string filledInIteration = forEachTemplate.Replace(keywordToReplace, fileName);
                filledInForeachRegion.Append(filledInIteration);
            }

            return filledInForeachRegion.ToString();
        }
    }

    static class DirectoryScanner
    {
        public static IEnumerable<string> GetDirectoryFilesFullPathsRecursively(string directoryWithFiles, string searchPattern)
        {
            string[] filesInFolder = Directory.GetFiles(directoryWithFiles, searchPattern);
            foreach (string file in filesInFolder)
            {
                yield return file;
            }

            string[] nestedFolders = Directory.GetDirectories(directoryWithFiles);
            foreach (string nestedFolder in nestedFolders)
            {
                var allFoldersFiles = GetDirectoryFilesFullPathsRecursively(nestedFolder, searchPattern);
                foreach (string nestedFile in allFoldersFiles)
                {
                    yield return nestedFile;
                }
            }
        }
    }
}
