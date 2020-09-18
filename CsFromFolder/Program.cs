using CommandLineParser.Exceptions;
using System;
using System.IO;
using System.Linq;

namespace CsFromFolder
{
    public class Program
    {
        public static void Main(string[] args)
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
                    relativeFilePaths: DirectoryScanner
                        .GetDirectoryFilesFullPathsRecursively(resourcesDirectory, searchPattern)
                        .Select(fileAbs => Path.GetRelativePath(resourcesDirectory, fileAbs))));

            Console.WriteLine(filledIn);
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
}
