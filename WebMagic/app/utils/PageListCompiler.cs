namespace WebMagic {
    class PageListCompiler {
        private string baseDirectory;

        public PageListCompiler(string baseDirectory) {
            this.baseDirectory = baseDirectory;
        }

        public void CompilePages(string[] inputPagesList) {
            string inputDirectory = GlobalPaths.ProjectFolder;
            string outputDirectory = GlobalPaths.OutputFolder;

            // Create the output directory if it doesn't exist
            if (!System.IO.Directory.Exists(outputDirectory)) {
                System.IO.Directory.CreateDirectory(outputDirectory);
            }

            foreach (string pagePath in inputPagesList) {
                Compiler1.Compile(pagePath, inputDirectory, outputDirectory);
            }
        }
    }

    public static class Compiler1 {
        public static void Compile(string pagePath, string inputDirectory, string outputDirectory) {
            string fileName = pagePath.Substring(pagePath.LastIndexOf("\\") + 1);
            string outputPath = outputDirectory + fileName;
            string textToWrite = "how are you?";

            // Write "how are you?" to the output file
            System.IO.File.WriteAllText(outputPath, textToWrite);
        }
    }
}
