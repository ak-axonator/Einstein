using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WebMagic
{
    public class GPTPromptsGenerator
    {
        private const string InputFilePath = @"E:\JK\GPTJsonPageGenFiles\GPTInput.jsonc";
        private const string OutputFilePath = @"E:\JK\GPTJsonPageGenFiles\GPTPageContent.jsonc";

        public void Generate()
        {
            var inputJson = File.ReadAllText(InputFilePath);
            inputJson = Regex.Replace(inputJson, @"^\s*//.*$", "", RegexOptions.Multiline);
            var input = JsonConvert.DeserializeObject<Input>(inputJson);
            var prompts = input.Prompts;
            var GPTPromptsFile = new GPTPromptFile();
            GPTPromptsFile.Prompts = new List<GPTPrompt>();

            foreach (var prompt in prompts)
            {
                if (prompt.Enabled == false)
                    continue;
                GPTPrompt gPTPrompt = new GPTPrompt();
                gPTPrompt.Prompt = GetPromptString(prompt, input);
                gPTPrompt.Section = prompt.Section;

                GPTPromptsFile.Prompts.Add(gPTPrompt);
            }
            var outputJson = JsonConvert.SerializeObject(GPTPromptsFile, Formatting.Indented);
            File.WriteAllText(OutputFilePath, outputJson);
        }

        private string GetPromptString(Prompt prompt, Input input)
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
                    promptString = $"Give {numberString} {prompt.What} for the app: {input.AppName}. Here is what the app does: {input.AppDescription}. {prompt.AdditionalPrompt}. Provide {expected_once_list}. And for each point provide: {expected_repeat_list}. Use the keywords '{keywords}' as much as possible in your response. Maximum length for headlines and descriptions should be 50 words. {json_format} {additionalNotes}";
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

    public class Input
    {
        public List<string> Keywords { get; set; }
        public string BackgroundContent { get; set; }
        public string AppName { get; set; }
        public string AppDescription { get; set; }
        public List<string> PageStart { get; set; }
        public List<string> PageEnd { get; set; }

        public List<Prompt> Prompts { get; set; }
    }

    public class Prompt
    {
        public string Type { get; set; }
        public int Number { get; set; }
        public string What { get; set; }
        public string AdditionalPrompt { get; set; }
        public List<string> ExpectedOnce { get; set; }
        public List<string> ExpectedRepeat { get; set; }
        public bool Enabled { get; set; }
        public bool UseOnlyWhat { get; set; }
        public string Section { get; set; }
    }
}
