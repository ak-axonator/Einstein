using System;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO; // Need to add reference to Microsoft.VisualBasic.dll

namespace WebMagic
{
    public class CSVProcessor
    {
        private string _filePath;
        ChatGPTAPI chatGPTAPI = new ChatGPTAPI();

        public CSVProcessor(string filePath)
        {
            _filePath = filePath;
        }

        public void ProcessCSV()
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            // Create a new TextFieldParser object
            TextFieldParser parser = new TextFieldParser(_filePath);

            // Set up the parser's properties
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            var i = 0;
            // Read the file and parse the data
            while (!parser.EndOfData)
            {
                // Read the current line
                string[] fields = parser.ReadFields();

                // Process the fields as needed
                Input csvInput = new Input();
                csvInput.Industry = fields[0].Trim();
                if (csvInput.Industry.Contains("Industry") || csvInput.Industry.Contains("Category"))
                    continue;
                csvInput.Category = fields[1].Trim();
                csvInput.AppName = fields[2].Trim().Replace("/","-");
                // csvInput.Keywords = fields[2].Trim().Split(',').ToList();
                var timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                Console.WriteLine(timestamp);
                Console.WriteLine($"Processing {csvInput.AppName}...");
                var generator = new GPTPromptsGenerator();
                try{
                    generator.Generate(csvInput);
                }
                catch(Exception e){
                    CommandProcessor.LogJsonParsingError(e, e.Message, fields.ToString());
                }
                // testGPTAPICalls();
                
                Console.WriteLine();
                i++;
                // string prompt = GeneratePrompt(title, category, subcategory);
                // GetGPTResponse(prompt);

                // string prompt2 = GeneratePrompt2(title, category, subcategory);
                // GetGPTResponse(prompt2);
            }

            // Close the parser
            parser.Close();

        }

        private string GeneratePrompt(string title, string category, string subcategory)
        {
            StringBuilder prompt = new StringBuilder();
            prompt.Append("Generate app description for mobile app called: ");
            prompt.Append(title);
            prompt.Append(". It is for ");
            prompt.Append(category);
            prompt.Append(". The app is related to ");
            prompt.Append(subcategory);
            prompt.Append(".");

            return prompt.ToString();
        }

        private string GeneratePrompt2(string title, string category, string subcategory)
        {
            StringBuilder prompt = new StringBuilder();
            prompt.Append("Rewrite the name in less than 10 words: ");
            prompt.Append(title);
            prompt.Append(". It is for ");
            prompt.Append(category);
            prompt.Append(". The app is related to ");
            prompt.Append(subcategory);
            prompt.Append(".");
            return prompt.ToString();
    }

    private void GetGPTResponse(string prompt)
    {

        string response = chatGPTAPI.GetResponse(prompt);
        Console.WriteLine("Response: " + response);
    }
}
}
