using System;
using System.IO;

namespace WebMagic
{
    public class PagesCompiler
    {
        public static void CompilePages(string inputDir, string outputDir)
        {
            if (!Directory.Exists(inputDir))
            {
                throw new DirectoryNotFoundException("Input directory does not exist.");
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string[] files = Directory.GetFiles(inputDir, "*.page", SearchOption.AllDirectories);

            foreach (string inputFile in files)
            {
                string outputFilePath = Path.Combine(outputDir, inputFile.Substring(inputDir.Length + 1));
                string outputDirectory = Path.GetDirectoryName(outputFilePath);

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                using (StreamReader reader = new StreamReader(inputFile))
                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // modify the line as needed
                        line = "i did this. are you ok?";
                        writer.WriteLine(line);
                    }
                }
            }
        }
    }
}
