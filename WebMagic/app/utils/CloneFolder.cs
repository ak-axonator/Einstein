using System;
using System.IO;
using System.Linq;

namespace WebMagic
{
    public class CloneFolder
    {
        private readonly string _inputPath;
        private readonly string _outputPath;
        private readonly string[] _ignoreFolders;

        public CloneFolder(string inputPath, string outputPath, string[] ignoreFolders)
        {
            _inputPath = inputPath;
            _outputPath = outputPath;
            _ignoreFolders = ignoreFolders;
        }

        public void Clone()
        {
            CloneDirectory(_inputPath, _outputPath);
        }

        private void CloneDirectory(string source, string destination)
        {
            var directory = new DirectoryInfo(source);
            var directories = directory.GetDirectories()
                .Where(d => !_ignoreFolders.Contains(d.Name)).ToList();

            if (!directories.Any())
            {
                Directory.CreateDirectory(destination);
            }
            else
            {
                foreach (var dir in directories)
                {
                    string destinationDir = Path.Combine(destination, dir.Name);
                    Directory.CreateDirectory(destinationDir);
                    CloneDirectory(dir.FullName, destinationDir);
                }
            }

            foreach (var file in directory.GetFiles())
            {
                file.CopyTo(Path.Combine(destination, file.Name), false);
            }
        }
    }
}
