using System.Text.RegularExpressions;
using KdlDotNet;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace WebMagic
{
    class Benefit
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
    class Program
    {
        static ComandProcessor processor;
        static void Main(string[] args)
        {
            //testChatGPTAPI();
            MainCompilationStart(args);
            //task.Wait();
            // InitGlobalPaths();
            //testGPTResponseFiletoKDL();
            testCSVAppNameRename();
            
            //testGPTGeneratePrompts();
            //testGPTAPICalls();
            // string response = testOpenAI();
            // testValidateResponse(response);
            // testBenefitsParsing(response);
            //testS3Uploading();

        }

        private static void testCSVAppNameRename()
        {
            string csvFileName = @"E:\JK\app30k.csv";
            var csv = new CSVProcessor(csvFileName);
            csv.ProcessCSV();
        }

        private static void testGPTResponseFiletoKDL()
        {
            string inputFilePath = @"E:\JK\GPTJsonPageGenFiles\GPTPageContentOutput.jsonc";
            string outputFilePath = @"E:\JK\GPTJsonPageGenFiles\GPTPageContentKDL.kdl";
            string promptGeneratorFilePath = @"E:\JK\GPTJsonPageGenFiles\GPTInput.jsonc";
            
            runGPTResponseFiletoKDL(inputFilePath, outputFilePath, promptGeneratorFilePath);
            
        }

        private static void runGPTResponseFiletoKDL(string inputFilePath, string outputFilePath, string promptGeneratorFilePath)
        {
            GPTResponseFiletoKDL.Run(inputFilePath, outputFilePath, promptGeneratorFilePath);
        }

        private static void testChatGPTAPI()
        {
            ChatGPTAPI chatGPTAPI = new ChatGPTAPI();
            string response = chatGPTAPI.GetResponse("give three challenges in building an ios app. Format the points in a json format: { [ {'headline': headline of point, 'description': description of point },]}");
            Console.WriteLine(response);
        }


        static void testGPTAPICalls()
        {
            string inputFilePath = @"E:\JK\GPTJsonPageGenFiles\GPTPageContent.jsonc";
            string outputFilePath = @"E:\JK\GPTJsonPageGenFiles\GPTPageContentOutput.jsonc";

            runOpenAICalls(inputFilePath, outputFilePath);
        }

        static void runOpenAICalls(string inputFilePath, string outputFilePath)
        {
            GPTPromptsRunner.Run(inputFilePath, outputFilePath);
        }
        private static void testGPTGeneratePrompts()
        {
            var generator = new GPTPromptsGenerator();
            generator.Generate();
        }

        private static void testValidateResponse(string response)
        {
            // Create an instance of the GPTResponseValidator class
            GPTResponseValidator validator = new GPTResponseValidator();

            // Validate the response
            bool isResponseValid = validator.ValidateBulletResponse(response);

            if (isResponseValid)
            {
                Console.WriteLine("The response is in the expected format.");
            }
            else
            {
                Console.WriteLine("The response is not in the expected format.");
            }
        }

        private static void testBenefitsParsing(string benefitsText)
        {
            var benefits = ParseBullets(benefitsText);
            foreach (var benefit in benefits)
            {
                Console.WriteLine("Title: " + benefit.Title);
                Console.WriteLine("Description: " + benefit.Description);
                Console.WriteLine();
            }
        }

        private static string testOpenAI()
        {
            OpenAI  ai = new OpenAI();
            string result = ai.GetResponse("give three challenges in building an ios app. Format the points in a json format: { [ {'headline': headline of point, 'description': description of point },]}");
            Console.WriteLine(result);
            return result;

            
            //  var apiKey = "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";
            // var conversation = new OpenAIConversation(apiKey);

            // // Call the GetResponseAsync method with a prompt
            // var response = await conversation.GetResponseAsync("What is the meaning of life?");
            // return response;
        }
        static List<Benefit> ParseBullets(string text)
        {
            var benefits = new List<Benefit>();
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(new[] { ": " }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    continue;
                }
                benefits.Add(new Benefit
                {
                    Title = parts[0].TrimStart().TrimEnd('.'),
                    Description = parts[1].TrimStart(),
                });
            }
            return benefits;
        }
        private static void testS3Uploading()
        {
            string foldername = @"E:\JK\KDLPages_WebsiteProject\SourceFiles\output\testings3upload";
            S3Uploader uploader = new S3Uploader("us-west-2", "axonator.co", foldername);

        //     // Get all files in the folder
        //     string[] files = Directory.GetFiles(foldername);

        //     // Create a list to hold the file names
        //     List<string> fileNames = new List<string>();

        //     // Loop through each file and add its name to the list
        //     foreach (string file in files)
        //     {
        //         fileNames.Add(Path.GetFileName(file));
        //     }

        //     uploader.UploadFiles(files);

        }

        private static void MainCompilationStart(string[] args)
        {
            processor = new ComandProcessor(args);
            processor.processCommand();
        }

        private static void InitGlobalPaths()
        {
            string system_folder = @"E:\JK\KDLPages_WebsiteProject\SystemFiles";
            string project_folder = @"E:\JK\KDLPages_WebsiteProject\SourceFiles";
            string output_folder = project_folder + @"\output\";

            GlobalPaths.SystemFolder = system_folder;
            GlobalPaths.ProjectFolder = project_folder;
            GlobalPaths.OutputFolder = output_folder;
        }

        public static void CopyFolder(string sourceFolderPath, string destinationFolderPath)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo sourceDir = new DirectoryInfo(sourceFolderPath);
            DirectoryInfo[] sourceSubDirs = sourceDir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            // Copy the files in the source directory to the destination directory.
            FileInfo[] sourceFiles = sourceDir.GetFiles();
            foreach (FileInfo file in sourceFiles)
            {
                string destinationFilePath = Path.Combine(destinationFolderPath, file.Name);
                file.CopyTo(destinationFilePath, true);
            }

            // Copy the subdirectories in the source directory to the destination directory.
            foreach (DirectoryInfo subDir in sourceSubDirs)
            {
                string destinationSubDirPath = Path.Combine(destinationFolderPath, subDir.Name);
                CopyFolder(subDir.FullName, destinationSubDirPath);
            }
        }

        //     foreach (string file in pageList)
        //     {
        //         Console.WriteLine("Compiling " + file + "...");
        //         KDLFile inputFile = new KDLFile(Path.Combine(GlobalPaths.ProjectFolder, file));
        //         c.Compile(inputFile, Path.Combine(GlobalPaths.OutputFolder, Path.GetFileNameWithoutExtension(file) + ".html"), GlobalPaths.OutputFolder + Path.GetFileName(file) + ".html.errors.txt");
        //     }
        // }

    }
}