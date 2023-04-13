using System.Text.RegularExpressions;
using KdlDotNet;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ImageMagic;

namespace WebMagic
{
    class Benefit
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
    class Program
    {
        static CommandProcessor processor;
        public static string NewPageName { get; set; }
        static void Main(string[] args)
        {
            processor = new CommandProcessor();
            processor.initGlobalPaths();
            //testChatGPTAPI();
            // MainCompilationStart(args);
            //task.Wait();
            // initGlobalPaths();
            // testGPTResponseFiletoKDL();
            testCSVAppNameRename();
            // GenerateAndRunAppArtifactPrompts("form");
            // GenerateAndRunAppArtifactPrompts("report");
            // GenerateAndRunAppArtifactPrompts("checklist");
            // GenerateAndRunAppArtifactPrompts("dashboard");
            // GenerateAndRunAppArtifactPrompts("audit_checklist");
            // GenerateAndRunAppArtifactPrompts("workflow");
            // GenerateAndRunAppArtifactPrompts("guidelines");
            // GenerateAndRunAppArtifactPrompts("standards");
            // GenerateAndRunAppArtifactPrompts("document");
            // testDynamicSVGRender();
            // testPDFFormRender();
            // testGPTGeneratePrompts();
            // testGPTAPICalls();
            // string response = testOpenAI();
            // testValidateResponse(response);
            // testBenefitsParsing(response);
            //testS3Uploading();
            // CreatePageFileFromOutputKDL();
            // testMDtoKDLParser();
            // ReplacePageBeginSections();

        }

        public static void ReplacePageBeginSections(){
            string pagesFolder = "/Users/arohikulkarni/Work/Website Project/SourceFiles/WebsiteSourceProject";
            string metaFile = "/Users/arohikulkarni/Downloads/meta_kdls.txt";

            // Read meta_kdls file
            Dictionary<string, string> pageBeginSections = new Dictionary<string, string>();
            List<string> _lines = File.ReadAllLines(metaFile).ToList();
            for(var i = 0; i < _lines.Count; i++){
                string line = _lines[i];
                if(line.Contains("mdFileName")){
                    string fileName = Path.GetFileNameWithoutExtension(line.Split(':')[1].Trim().Replace('/', '_'));
                    string beginSection = string.Join("\n", _lines.GetRange(i + 1, 5));
                    pageBeginSections[fileName] = beginSection;
                    // break;
                }
            }

            // Find all .page files recursively
            List<string> pageFiles = Directory.GetFiles(pagesFolder, "*.page", SearchOption.AllDirectories).ToList();

            foreach (string pageFile in pageFiles)
            {
                // Get file name without extension and folder path
                string fileName = Path.GetFileNameWithoutExtension(pageFile);
                string folderPath = Path.GetDirectoryName(pageFile.Substring(pagesFolder.Length + 1));

                // Find corresponding md file name
                // string mdFileName = fileName.Replace('_', '/') + ".md";
                // if (!string.IsNullOrEmpty(folderPath))
                // {
                //     mdFileName = Path.Combine(folderPath, mdFileName);
                // }

                // Check if md file name is present in meta file
                if (pageBeginSections.TryGetValue(fileName, out string beginSection))
                {
                    // Replace page begin section in page file
                    string[] lines = File.ReadAllLines(pageFile);
                    int index = Array.FindIndex(lines, line => line.Contains("page_begin_section"));
                    if (index != -1 && !lines[index].Contains("page_begin_section {"))
                    {
                        lines[index] = beginSection;
                        File.WriteAllLines(pageFile, lines);
                    }
                }
            }
        }

        private static void testMDtoKDLParser()
        {
            string folderPath = "/Users/arohikulkarni/Work/Website Project/Old Website/md files";

            // var input = "What is the Purpose of a Control Plan in Quality Management?\n------------------------------------------------------------\n\nA quality control plan aims to establish a framework for preventing defects and non-conformities in a product or service.\n\n* It identifies key characteristics of the process and establishes methods for monitoring and measuring those characteristics.\n* It includes process steps, key process inputs and outputs, critical-to-quality (CTQ) characteristics, control methods, control limits, and corrective actions.\n* It establishes a control plan that ensures consistent production of quality products or services by identifying areas for improvement and reducing the risk of defects and non-conformities.\n\nWhat are the Three Types of Control Plans in Quality Management?\n----------------------------------------------------------------\n\nThree types of control plans are used in quality management:\n\nPrototype Control Plan\n----------------------\n\nThis type of control plan is used in the early stages of development when a product is still in the prototype phase. It outlines the dimensions, materials, and performance tests necessary for developing a prototype. \n\nFor example, a company developing a new smartphone may create a prototype control plan to ensure that the phone’s dimensions, materials, and performance tests meet the desired standards.\n\nPre-Launch Control Plan\n-----------------------\n\nOnce a product prototype is complete, a pre-launch control plan is used to ensure the product is ready for full production. It includes dimension measurements, materials, and performance tests conducted after the prototype phase. \n\nFor example, a company producing a new car may use a pre-launch control plan to test the vehicle’s braking system, acceleration, and fuel efficiency before the car is approved for full production.\n\nProduction Control Plan\n-----------------------\n\nA production control plan is used when a product is in total production. It includes characteristics, process controls, tests, and measurements throughout production. \n\nFor example, a company manufacturing electronic devices may use a production control plan to ensure that the products are consistent in quality, performance, and appearance throughout the production process.\n\nWhat is a Control Plan Format?\n------------------------------\n\nThere is no specific format that a control plan must follow, but generally, it includes the following information:\n\n| Control Plan Information | Description |\n| --- | --- |\n| Header Information | Title of the control plan, date of creation, and revision number |\n| Process Steps | Detailed description of process steps to identify potential quality issues and ensure consistent production. It may include process flow diagrams, process maps, and process descriptions. |\n| Key Process Inputs and Outputs | List of key inputs and outputs for each process step to ensure proper measurement and monitoring. |\n| Critical-to-Quality (CTQ) Characteristics | Identification of the most important characteristics directly affecting customer satisfaction. The control plan outlines acceptable limits and measurements for each CTQ characteristic. |\n| Control Methods | Outline of methods used to ensure the process operates within defined control limits, such as visual inspections, statistical process control, or other monitoring methods. |\n| Control Limits | Identification of acceptable ranges for each key input and output in the control plan. This allows for early detection of issues and timely corrective actions. |\n| Corrective Actions | Plan for corrective actions in case of deviations from the control limits, which may include stopping production, making adjustments to the process, or re-inspecting the product. |\n| Responsibilities | Identify whois responsible for each step of the process and who is responsible for monitoring and controlling the quality of the product or service |";
            List<string> files = new List<string>();
            // get all md files in the folder and convert them to kdl
            
            // string inputFilePath = @"/Users/arohikulkarni/Work/Website Project/Old Website/md files/home.md";
            // files.Add(inputFilePath);
            files.Add(@"/Users/arohikulkarni/Work/Website Project/Old Website/md files/micro_app_store_safety_audit_checklist_app_.md");
            // files.Add(@"/Users/arohikulkarni/Work/Website Project/Old Website/md files/asset_performance_management_software_.md");
            // files.Add(@"/Users/arohikulkarni/Work/Website Project/Old Website/md files/blog_data_collection_process_.md");
            // files.Add(@"/Users/arohikulkarni/Work/Website Project/Old Website/md files/mobile_apps_for_manufacturing_industry_.md");
            // foreach (var file in files)
            foreach (var file in Directory.GetFiles(folderPath, "*.md"))
            {
                var parser = new TextParser();
                try{
                    parser.Convert(file);
                }
                catch(Exception e){
                    CommandProcessor.LogJsonParsingError(e, e.Message, file);
                }
            }
        }

        private static void testPDFFormRender(){
            string htmlFilePath = "/Users/arohikulkarni/Work/Website Project/SourceFiles/FormHTMLs/Forklift inspection checklist.html";
            string pdfFilePath = HtmlToPdfConverter.Convert(htmlFilePath);
        }

        private static void testDynamicSVGRender(){
            ImageMagic.Program.testDynamicSVGRender();
        }

        private static void testCSVAppNameRename()
        {
            string csvFileName = @"/Users/arohikulkarni/Downloads/all app names with categories and industries reordered - Manufacturing.csv";
            var csv = new CSVProcessor(csvFileName);
            try{
                csv.ProcessCSV();
            }
            catch(Exception e){
                CommandProcessor.LogJsonParsingError(e, e.Message, csvFileName);
            }
        }

        // private static void testGPTGeneratePrompts()
        // {
        //     var generator = new GPTPromptsGenerator();
        //     generator.Generate();
        //     testGPTAPICalls();
        // }
        public static void generatePrompts(Input input)
        {            
            var generator = new GPTPromptsGenerator();
            generator.Generate(input);
            testGPTAPICalls();
        }
        public static void GenerateAndRunAppArtifactPrompts(string artifact)
        {            
            var generator = new GPTPromptsGenerator();
            try{
                generator.GenerateAppArtifactPrompts(artifact);
            }
            catch(Exception e){
                CommandProcessor.LogJsonParsingError(e, e.Message, artifact);
            }
            // testGPTAPICalls();
        }
        static void testGPTAPICalls()
        {
            string inputFilePath = Path.Combine(GlobalPaths.GPTFolder,"GPTPageContent.jsonc");
            string outputFilePath = Path.Combine(GlobalPaths.GPTFolder,"GPTPageContentOutput.jsonc");

            runOpenAICalls(inputFilePath, outputFilePath);
        }

        static void runOpenAICalls(string inputFilePath, string outputFilePath)
        {
            GPTPromptsRunner.Run(inputFilePath, outputFilePath);
            
            // testGPTResponseFiletoKDL();
        }

        // private static void GenerateArtifactPrompts(){
        //     string inputFilePath = Path.Combine(GlobalPaths.GPTFolder,"GPTPageContent.jsonc");
        //     var generator = new GPTPromptsGenerator();
        //     generator.ExtractAppArtifactsList(inputFilePath);
        //     testGPTAPICalls();
        // }

        private static void testGPTResponseFiletoKDL()
        {
            string inputFilePath = Path.Combine(GlobalPaths.GPTFolder,"GPTPageContentOutput.jsonc");
            string outputFilePath = Path.Combine(GlobalPaths.GPTFolder,"GPTPageContentKDL.jsonc");
            string promptGeneratorFilePath = Path.Combine(GlobalPaths.GPTFolder,"GPTInput.jsonc");

            runGPTResponseFiletoKDL(inputFilePath, outputFilePath, promptGeneratorFilePath);
            
        }
        private static void runGPTResponseFiletoKDL(string inputFilePath, string outputFilePath, string promptGeneratorFilePath)
        {
            GPTResponseFiletoKDL.Run(inputFilePath, outputFilePath, promptGeneratorFilePath);
            CreatePageFileFromOutputKDL(outputFilePath);
        }

        public static void CreatePageFileFromOutputKDL(string outputFilePath = "")
        {
            // Read the contents of the output file
            string _outputFilePath = outputFilePath == "" ? Path.Combine(GlobalPaths.ProjectFolder, "new_page.page") : outputFilePath;
            string contents = File.ReadAllText(_outputFilePath);
            string _fileName = (NewPageName != null && NewPageName != "" ? NewPageName : "new_page").ToLower().Replace(" ", "_");

            // Create a new file with the specified name in the project folder
            string pageFilePath = Path.Combine(GlobalPaths.ProjectFolder, _fileName+".page");
            File.WriteAllText(pageFilePath, contents);

            Console.WriteLine($"New page {_fileName} is created!");

            processor = new CommandProcessor();
            processor.processCommand();

            Console.WriteLine($"Hurray!!! Your new page {_fileName} is live now!");

        }



        private static void testChatGPTAPI()
        {
            ChatGPTAPI chatGPTAPI = new ChatGPTAPI();
            string response = chatGPTAPI.GetResponse("give three challenges in building an ios app. Format the points in a json format: { [ {'headline': headline of point, 'description': description of point },]}");
            Console.WriteLine(response);
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
            processor = new CommandProcessor(args);
            try{
                processor.processCommand();
            }
            catch(Exception e){
                CommandProcessor.LogJsonParsingError(e, e.Message);
            }
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