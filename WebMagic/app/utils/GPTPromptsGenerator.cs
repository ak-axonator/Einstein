using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebMagic
{
    public class GPTPromptsGenerator
    {
        private string InputFilePath = Path.Combine(GlobalPaths.GPTFolder,"ArtifactPrompts","GPTInputAppDetails.jsonc");
        private string OutputFilePath = Path.Combine(GlobalPaths.GPTFolder,"GPTPageContent.jsonc");
        public string BackgroundContent = "Axonator is a field workflow automation platform that provides automation of mobile forms, workflows, reports, dashboards, scheduling tasks, integration with third party software in a faster and easier way without coding.";
        public string AxoBenefits = "{\"Original_benefits\": [\"Tailor forms to specific needs\", \"Streamline data collection process\", \"Streamline checklist process\", \"Access forms without connectivity\", \"Ensure accuracy and completeness\", \"Collect multimedia information easily\", \"Keep data up-to-date\", \"Enhanced communication within team\", \"Record important conversations easily\", \"Visual information for better understanding\", \"Dynamic, error-free, user-friendly forms\", \"Identify issues, improve compliance\", \"Efficient and accurate record-keeping\", \"Clear record of agreement/consent\", \"Simplified, faster and accurate access to forms\", \"Perform SOPs, reduce field user training\", \"Create unique business processes\", \"Efficiently manage approval processes\", \"Receive instant workflow status updates\", \"Automate complex business processes\", \"Customize workflows for specific needs\", \"Automate and customize workflows dynamically\", \"Collaborate and improve performance with shared reports\", \"Visualize and optimize business processes\", \"Create custom workflows for specific tasks\", \"Ensure timely completion of tasks\", \"Track progress and identify issues\", \"Streamline task assignment process\", \"Assign tasks as needed\", \"Ensure timely completion of forms\", \"View tasks in one place\", \"Tailor reports to specific needs\", \"Save time and reduce errors\", \"Improve efficiency and accuracy\", \"Reinforce brand identity\", \"Analyze data easily\", \"Improve communication between teams\", \"Customized reporting for your business\", \"Digitally automated reports\", \"Get the data you need\", \"Improve business outcomes\", \"Track progress and identify trends\", \"Relevant information at-a-glance\", \"Make informed decisions easily\", \"Understand data quickly\", \"Streamline data sharing process\", \"Connect apps and data sources\", \"Tailor integrations to specific needs\", \"Get the integrations you need\", \"Streamline data management\", \"Find information quickly\", \"Share data easily\", \"Save time and reduce errors\", \"Modify data easily\", \"Access data from anywhere\", \"Create custom mobile apps\", \"Create custom forms easily\", \"Streamline complex business processes\", \"Generate custom reports quickly\", \"Manage tasks efficiently\", \"Monitor important tasks easily\", \"Create custom floor plans\", \"Design floor plans easily\", \"Locate assets quickly\", \"Navigate with ease\", \"Easily locate specific assets\", \"View asset details efficiently\"]}";
        public string AxoFeatures = "{\"Original_features\": [\"Build custom forms\", \"Convert paper forms into mobile apps\", \"Convert paper checklists into mobile apps\", \"Work offline\", \"Validate data\", \"Capture rich data\", \"Auto sync\", \"Annotate captured pictures\", \"Capture Audio \", \"Capture videos\", \"Advanced form scripting to control form behavior\", \"A person using audit, inspection form\", \"A person capturing data\", \"Capture person's signature\", \"Scan QR code to open form\", \"Fill checklist\", \"Build custom workflows\", \"Build approval workflows\", \"Send notifications\", \"Automate multi-step multi-user workflows\", \"Add custom processing steps to  workflows\", \"Advanced workflow scripting to control workflow behavior\", \"Share reports with teams\", \"Workflow concept\", \"Specific workflow\", \"Schedule regular tasks\", \"Monitor tasks performed\", \"Automatically assign tasks\", \"Manually assign tasks\", \"Set reminders for form completion\", \"Calendar integration\", \"Build custom reports\", \"Send reports automaticallly\", \"Generate and share reports automatically\", \"Brand your reports\", \"Person checking report\", \"Generate annotated picture reports\", \"Specific reports\", \"Reports concept\", \"Build custom dashboards\", \"Make data-driven decisions\", \"Generate performance dashboards\", \"Specific dashboard\", \"Person with dashboard\", \"Analyze and visualize data\", \"Send data to third party apps\", \"integrations concept\", \"custom integrations\", \"specific integrations\", \"Centralized data in one place\", \"Search data\", \"Export data\", \"Import data\", \"Edit data\", \"Person using web app\", \"App Builder\", \"Form Builder\", \"Workflow Builder\", \"Report Builder\", \"Scheduler\", \"Watcher\", \"Floor map builder\", \"Person using floor map builder\", \"Find assets on floor map\", \"Find location using floor map\", \"Search assets on floor map\", \"Select assets and check properties\"]}";
        public GPTPromptsGenerator(){
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string gpt_folder = configuration.GetValue<string>("gpt_folder");
            // InputFilePath = Path.Combine(gpt_folder,"GPTInput.jsonc");
            // OutputFilePath = Path.Combine(gpt_folder,"GPTPageContent.jsonc");
        }
        public void Generate(Input _csvInput)
        {
            // if input.appname is empty or null, then return
            if (string.IsNullOrEmpty(_csvInput.AppName))
                return;
            string outputFilePath = Path.Combine(GlobalPaths.LogFolder, "GPTAppPageContents - " + _csvInput.AppName + ".jsonc");
            //if outputfile exists, skip it
            if (File.Exists(outputFilePath))
            {
                Console.WriteLine($"Skipping {outputFilePath}...");
                return;
            }
            Console.WriteLine($"Generating App Details prompt for {_csvInput.AppName}...");
            var timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            Console.WriteLine(timestamp);
            var artifact = "app_details";
            try{
                var inputFilePath = Path.Combine(GlobalPaths.GPTFolder, "ArtifactPrompts", "GPTInput" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(artifact.Replace("_", "")) + ".jsonc");
                var inputJson = File.ReadAllText(inputFilePath);
                inputJson = Regex.Replace(inputJson, @"^\s*//.*$", "", RegexOptions.Multiline);
                var input = JsonConvert.DeserializeObject<Input>(inputJson);
                if(_csvInput != null){
                    input.AppName = _csvInput.AppName;
                    input.Industry = _csvInput.Industry;
                    input.Category = _csvInput.Category;
                }
                var appDetailsPrompt = input.Prompts[0];
                if (appDetailsPrompt.Enabled == false)
                    return;
                Program.NewPageName = input.AppName;
                string promptString = GetPromptString(appDetailsPrompt, input);
                Console.WriteLine($"Generated {artifact} prompt for {input.AppName}...");
                // Console.WriteLine(promptString);
                GPTPromptsRunner.Run(promptString, outputFilePath);


                // generate and benefits prompt, and add the benefits to the app details json file
                


                GenerateAndAddBenefitsAndFeatures(_csvInput,outputFilePath);

            }
            catch(Exception e){
                CommandProcessor.LogJsonParsingError(e, e.Message, artifact + " -- " + _csvInput.AppName);
            }
            // File.WriteAllText(OutputFilePath, outputJson);
            // File.WriteAllText(LogFilePath, outputJson);

            // var GPTPromptsFile = new GPTPromptFile();
            // GPTPromptsFile.Prompts = new List<GPTPrompt>();
            // foreach (var prompt in prompts)
            // {
                // GPTPrompt gPTPrompt = new GPTPrompt();
                // gPTPrompt.Prompt = GetPromptString(prompt, input);
                // gPTPrompt.Artifact = prompt.Artifact;

                // GPTPromptsFile.Prompts.Add(gPTPrompt);
            // }
        }
        public void GenerateAndAddBenefitsAndFeatures(Input _input, string outputFilePath){
            var inputFilePath = Path.Combine(GlobalPaths.GPTFolder, "ArtifactPrompts", "GPTInputAppBenefitsAndFeatures.jsonc");
            var inputJson = File.ReadAllText(inputFilePath);
            inputJson = Regex.Replace(inputJson, @"^\s*//.*$", "", RegexOptions.Multiline);
            var input = JsonConvert.DeserializeObject<Input>(inputJson);
            if(_input != null){
                input.AppName = _input.AppName;
                input.Industry = _input.Industry;
                input.Category = _input.Category;
                input.ArtifactName = _input.ArtifactName;
            }
            var benefitsPrompt = input.Prompts[0];
            if (benefitsPrompt.Enabled == false)
                return;
            string promptString = GetPromptString(benefitsPrompt, input);
            Console.WriteLine($"Generated benefits prompt for {input.AppName}...");

            // Read the original JSON file
            JObject originalJson = JObject.Parse(File.ReadAllText(outputFilePath));

            // Call GPTPromptsRunner.Run(promptString, outputFilePath) here
            GPTPromptsRunner.Run(promptString, outputFilePath);

            // Read the updated JSON file after calling GPTPromptsRunner.Run
            JObject updatedJson = JObject.Parse(File.ReadAllText(outputFilePath));

            // Update the original JSON with the combined contents
            originalJson.Merge(updatedJson);

            // Save the updated JSON to the output file
            File.WriteAllText(outputFilePath, originalJson.ToString());
        }
        public void GenerateSamplePreviewMD (Input _input, string inputFilePath, string outputFilePath)
        {
            var inputJson = File.ReadAllText(inputFilePath);
            inputJson = Regex.Replace(inputJson, @"^\s*//.*$", "", RegexOptions.Multiline);
            var input = JsonConvert.DeserializeObject<Input>(inputJson);
            if(_input != null){
                input.AppName = _input.AppName;
                input.Industry = _input.Industry;
                input.Category = _input.Category;
                input.ArtifactName = _input.ArtifactName;
            }
            var sampleDocPrompt = input.Prompts[0];
            if (sampleDocPrompt.Enabled == false)
                return;
            string promptString = GetPromptString(sampleDocPrompt, input);
            Console.WriteLine($"Generated sample doc prompt for {input.AppName}...");

            // Call GPTPromptsRunner.Run(promptString, outputFilePath) here
            GPTPromptsRunner.Run(promptString, outputFilePath);

        }
        public void GenerateAppArtifactPrompts(string artifact)
        {
            var GPTOutputFilesPath = GlobalPaths.LogFolder;
            Console.WriteLine($"Parsing app details from {GPTOutputFilesPath} for {artifact}(s)...");
            // loop through all files starting wiht "GPTAppPageContents - " and ending with ".jsonc"
            // var files = Directory.GetFiles(GlobalPaths.LogFolder, "GPTAppPageContents - *.jsonc");
            var files = Directory.GetFiles(GlobalPaths.LogFolder, "GPTAppPageContents - Drill bit manufacture process tracking.jsonc");
            foreach (var file in files)
            {
                var app_details = File.ReadAllText(file);
                app_details = Regex.Replace(app_details, @"^\s*//.*$", "", RegexOptions.Multiline);
                var appDetails = JsonConvert.DeserializeObject<AppDetails>(app_details);
                
                // generate prompt for each "artifact" in appDetails and run it
                    foreach (var artifact_name in getAppArtifacts(appDetails, artifact))
                    {
                        try{
                            // var currentPath = Directory.GetCurrentDirectory();
                            // run artifact prompt and store response in the artifact's json file in logfolder
                            var filePrefix = artifact.ToUpper().Replace("_", "-").Replace(" ", "-");
                            var fileName = artifact_name.ToLower().Replace("_", "-").Replace(" ", "-");
                            var appName = appDetails.Name.ToLower().Replace("_", "-").Replace(" ", "-");
                            var outputFilePath = Path.Combine(GlobalPaths.LogFolder, filePrefix + "-" + "appname-" + appName + "-artifactname-" + fileName + getArtifactExtension(artifact));
                            var outputPreviewFilePath = Path.Combine(GlobalPaths.LogFolder, filePrefix + "-" + "appname-" + appName + "-artifactname-" + fileName + ".md");
                            if (File.Exists(outputFilePath)){
                                Console.WriteLine($"Skipping {artifact_name} {artifact} in {appDetails.Name}...");
                                continue;
                            }
                            Console.WriteLine($"Generating prompt for {artifact_name} {artifact} in {appDetails.Name} app...");
                            var GPTInputFilePath = Path.Combine(GlobalPaths.GPTFolder, "ArtifactPrompts", "GPTInput" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(artifact.Replace("_", "")) + ".jsonc");
                            var GPTInputSampleFilePath = Path.Combine(GlobalPaths.GPTFolder, "ArtifactPrompts", "GPTInput" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(artifact.Replace("_", "")) + "Sample.jsonc");
                            var GPTInputJson = File.ReadAllText(GPTInputFilePath);
                            GPTInputJson = Regex.Replace(GPTInputJson, @"^\s*//.*$", "", RegexOptions.Multiline);
                            var input = JsonConvert.DeserializeObject<Input>(GPTInputJson);
                            input.AppName = artifact_name;
                            input.Industry = appDetails.Industry;
                            input.Category = appDetails.Category;
                            input.ArtifactName = artifact;

                            var formPrompt = input.Prompts[0];
                            if (formPrompt.Enabled == false)
                                return;
                            string promptString = GetPromptString(formPrompt, input);
                            Console.WriteLine($"Generated prompt for {artifact_name} {artifact} in {input.AppName}...");
                            // Console.WriteLine(promptString);

                            GPTPromptsRunner.Run(promptString, outputFilePath);
                            if(artifact == "form" || artifact == "workflow" || artifact == "checklist" || artifact == "audit_checklist" || artifact == "dashboard" || artifact == "report"){
                                GenerateAndAddBenefitsAndFeatures(input,outputFilePath);
                            }
                            //generate md files for sample preview for media artifacts
                            if(artifact == "report" || artifact == "dashboard" || artifact == "guidelines" || artifact == "standards" || artifact == "document"){
                                GenerateSamplePreviewMD(input,GPTInputSampleFilePath,outputPreviewFilePath);
                            }
                        }
                        catch(Exception e){
                            CommandProcessor.LogJsonParsingError(e, e.Message, artifact + " -- " + file);
                        }
                    }
                // var outputJson = JsonConvert.SerializeObject(GPTResponseFile, Formatting.Indented);
                // // get app name after "GPTAppPageContents - " and before ".jsonc"
                // var appName = Path.GetFileNameWithoutExtension(file).Substring(20);
                // string LogFilePath = Path.Combine(GlobalPaths.LogFolder, appName + " - " + artifact + ".jsonc");
                // File.WriteAllText(LogFilePath, outputJson);
                // Console.WriteLine($"Generated prompts in {LogFilePath}");
            }

        }
        //get artifact extension using switch case, if report, dashboard, guidelines or standards, then return ".md", else ".jsonc"
        private string getArtifactExtension(string artifact){
            string extension = ".jsonc";
            // switch (artifact)
            // {
            //     case "report":
            //         extension = ".md";
            //         break;
            //     case "dashboard":
            //         extension = ".md";
            //         break;
            //     case "guidelines":
            //         extension = ".md";
            //         break;
            //     case "standards":
            //         extension = ".md";
            //         break;
            //     case "document":
            //         extension = ".md";
            //         break;
            //     default:
            //         break;
            // }
            return extension;
        }
        private List<string> getAppArtifacts(AppDetails app_details, string artifact){
            List<string> artifacts = new List<string>();
            switch (artifact)
            {
                case "form":
                    artifacts = app_details.Form_Names;
                    break;
                case "checklist":
                    artifacts = app_details.Checklist_Names;
                    break;
                case "report":
                    artifacts = app_details.Report_Names;
                    break;
                case "dashboard":
                    artifacts = app_details.Dashboard_Names;
                    break;
                case "workflow":
                    artifacts = app_details.Workflows.Workflows.Select(x=>x.Workflow_Title).ToList();;
                    break;
                case "audit_checklist":
                    artifacts = app_details.AuditChecklist_Names;
                    break;
                case "guidelines":
                    artifacts = app_details.Guidelines_Names;
                    break;
                case "standards":
                    artifacts = app_details.Standards_Names;
                    break;
                case "document":
                    artifacts = app_details.Document_Names;
                    break;
                default:
                    break;
            }
            return artifacts;
        }
        private string GetPromptString(Prompt promptInput, Input input)
        {
            // Get prompt string by combining the JSONKey and the JSONValue for each obj in "GiveContentFor" array in the prompt
            // string promptString = string.Empty;
            StringBuilder prompt = new StringBuilder();

            // Background
            prompt.Append("Background: ");
            prompt.Append(BackgroundContent);
            prompt.AppendLine();

            // Consider industry and category
            prompt.Append("Consider industry: ");
            prompt.Append(input.Industry);
            prompt.Append(", Consider category: ");
            prompt.Append(input.Category);
            prompt.AppendLine();

            // Consider app or give artifact
            prompt.Append(promptInput.PrefixInstruction);
            prompt.Append(input.AppName); // or any artifact's name
            prompt.AppendLine();

            // if length of promptInput.GiveContentFor is more than 0, then add the following
            if (promptInput.GiveContentFor.Count > 0)
            {
                foreach (var item in promptInput.GiveContentFor)
                {
                    prompt.AppendLine($"\tJSON Key: {item.JSONKey}, JSON Value: {item.JSONValue}");
                }
            }
            prompt.AppendLine("More Instructions:");
            foreach (var instruction in promptInput.MoreInstructions)
            {
                prompt.AppendLine($"\t{instruction}");
            }
            if (prompt.ToString().Contains("industry_specific_features")){
                if (input.ArtifactName == "form" || input.ArtifactName == "workflow" || input.ArtifactName == "checklist" || input.ArtifactName == "audit_checklist"){
                    prompt.Replace("{artifact_name}", input.ArtifactName);
                }
                else{
                    prompt.Replace("{artifact_name}", "app");
                }
                prompt.AppendLine(AxoFeatures);
            }
            // if (prompt.ToString().Contains("industry_specific_benefits"))
            //     prompt.AppendLine(AxoBenefits);
            return prompt.ToString();
        }
        private string GetPromptString_old(Prompt prompt, Input input)
        {
            string numberString = prompt.Number.ToString();
            string keywords = String.Join(", ", input.Keywords.Select(x => x.ToString()).ToArray());

            string expected_once_list = string.Empty;
            string expected_once_json = string.Empty;
            string json_format = string.Empty;
            if (prompt.ExpectedOnce != null)
            {
                expected_once_list += String.Join(", ", prompt.ExpectedOnce.Select(x => x.ToString()).ToArray());
                expected_once_json += String.Join(", ", prompt.ExpectedOnce.Select(x => $"'{x}': {x}").ToArray());
                json_format +=  $"'Header': {{{expected_once_json}}}";
            }
            string expected_repeat_list = string.Empty;
            string expected_repeat_json = string.Empty;
            if (prompt.ExpectedRepeat != null)
            {
                expected_repeat_list += String.Join(", ", prompt.ExpectedRepeat.Select(x => x.ToString()).ToArray());
                expected_repeat_json += String.Join(", ", prompt.ExpectedRepeat.Select(x => $"'{x}': {x}").ToArray());
                json_format += $", 'Points': '[{{{expected_repeat_json}}},]'";
            }

            var promptString = string.Empty;

            if (prompt.UseOnlyWhat)
            {
                var obj = new { Input = input, Prompt = prompt };
                promptString = ReplaceEnclosedValues(prompt.What, obj);
                promptString += prompt.AdditionalPrompt;
            }
            else
            {
                if (json_format != string.Empty)
                {
                    json_format = $"The response should be in a json format described here: {{{json_format}}}.";
                }
                string additionalNotes = "Note only give JSON and no extra text. All titles, subtitle and descriptions must be less than 45 words. For JSON use double quotes only.";
                if (prompt.Type == "SinglePointPrompt")
                    promptString = $"Give {prompt.What} for the app: {input.AppName}. Here is what the app does: {input.AppDescription}. {prompt.AdditionalPrompt}. Use the keywords '{keywords}' as much as possible in your response. {json_format}. {additionalNotes}";
                else if (prompt.Type == "MultiPointPrompt")
                    promptString = $"Give {numberString} {prompt.What} for the app: {input.AppName}. Here is what the app does: {input.AppDescription}. {prompt.AdditionalPrompt}. Provide {expected_once_list}. And for each point provide: {expected_repeat_list}. Use the keywords '{keywords}' as much as possible in your response. Maximum length for headlines, subheadlines and descriptions should be 50 words. {json_format} {additionalNotes}";
            }
            return promptString;
        }
        public static string ReplaceEnclosedValues(string inputString, object obj)
        {

            //string input = "This is a {xyz.abc[0]} and {xyz.abc} example.";

            Regex regex = new Regex(@"\{(\w+)\.(\w+)(?:\[(\d+)\])?\}");
            MatchCollection matches = regex.Matches(inputString);

            foreach (Match match in matches)
            {
                string objectName = match.Groups[1].Value;
                string propertyName = match.Groups[2].Value;
                int index = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : -1; // Use -1 to indicate no index specified
                PropertyInfo property = obj.GetType().GetProperty(objectName); // Get the property with the matching name
                    if (property != null)
                    {
                        object value = property.GetValue(obj); // Get the value of the property
                        
                        if (value != null)
                        {
                            PropertyInfo property2 = value.GetType().GetProperty(propertyName);
                            object value2 = value.GetType().GetProperty(propertyName).GetValue(value);                            
                            if (index != -1)
                            {
                                value2 =  value2.GetType().GetProperty("Item").GetValue(value2, new object[] { index });
                            }
                            else
                            {
                                value2 = value2.ToString();
                            }
                            inputString = inputString.Replace(match.Value, value2.ToString()); // Replace the match with the property value
                        }
                    }
                // Validate the input
                    Console.WriteLine($"ObjectName: {objectName}, PropertyName: {propertyName}, Index: {index}");
            }
            return inputString;
        }
    }

    public class GiveContentFor
    {
        public string JSONKey { get; set; }
        public string JSONValue { get; set; }
    }

    public class Input
    {
        public string BackgroundContent { get; set; }
        public string AppName { get; set; }
        public string ArtifactName { get; set; }
        public string Industry { get; set; }
        public string Category { get; set; }
        public List<string> Keywords { get; set; }
        public string AppDescription { get; set; }
        public List<string> PageStart { get; set; }
        public List<string> PageEnd { get; set; }

        public List<Prompt> Prompts { get; set; }
    }

    public class Prompt
    {
        public string Type { get; set; }
        public string Artifact { get; set; }
        public string PrefixInstruction { get; set; }
        public List<GiveContentFor> GiveContentFor { get; set; }
        public int Number { get; set; }
        public string What { get; set; }
        public string AdditionalPrompt { get; set; }
        public List<string> MoreInstructions { get; set; }
        public List<string> ExpectedOnce { get; set; }
        public List<string> ExpectedRepeat { get; set; }
        public bool Enabled { get; set; }
        public bool UseOnlyWhat { get; set; }
        public string Section { get; set; }
    }
}
