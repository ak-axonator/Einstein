using System;
using System.IO;
using System.Text;

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
            


            using (var reader = new StreamReader(_filePath))
            {
                // Read the header line
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    string title = values[0].Trim();
                    string category = values[1].Trim();
                    string subcategory = values[2].Trim();

                    string prompt = GeneratePrompt(title, category, subcategory);
                    GetGPTResponse(prompt);

                    string prompt2 = GeneratePrompt2(title, category, subcategory);
                    GetGPTResponse(prompt2);
                }
            }
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
