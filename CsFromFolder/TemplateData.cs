using System.Collections.Generic;
using System.Linq;

namespace CsFromFolder
{
    sealed class TemplateData
    {
        private IEnumerable<string> relativeFilesPaths;

        public IEnumerable<string> RelativeFilesPaths
        {
            get => relativeFilesPaths.ToArray();
            private set => relativeFilesPaths = value.ToArray();
        }

        public TemplateData(IEnumerable<string> relativeFilePaths)
        {
            RelativeFilesPaths = relativeFilePaths.ToArray();
        }
    }
}
