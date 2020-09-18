using CommandLineParser.Arguments;
using System.IO;

namespace CsFromFolder
{
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
}
