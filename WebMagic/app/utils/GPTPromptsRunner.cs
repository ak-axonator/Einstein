using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace WebMagic
{
    public class GPTPromptsRunner
    {
        public static void Run(string inputFilePath, string outputFilePath)
        {
            ChatGPTAPI  ai = new ChatGPTAPI();
            
            // Load JSON from file
            string jsonString = File.ReadAllText(inputFilePath);
            //JsonDocument document = JsonDocument.Parse(jsonString);
            var input = JsonConvert.DeserializeObject<GPTPromptFile>(jsonString);

            var outputContent = new GPTResponseFile();
            outputContent.Responses = new List<GPTResponse>();

            foreach (var gptPrompt in input.Prompts)
            {
                string prompt = gptPrompt.Prompt;
                Console.WriteLine("Running prompt: "+ prompt);
                string result = ai.GetResponse(prompt);
                Console.WriteLine("ChatGPT Response: " + result);
                GPTResponse response = new GPTResponse();
                response.Section = gptPrompt.Section;
                response.Response = result;
                
                outputContent.Responses.Add(response);
                System.Threading.Thread.Sleep(5000);
            }
            // Write output JSON to file
            var outputJson = JsonConvert.SerializeObject(outputContent, Formatting.Indented);
             File.WriteAllText(outputFilePath, outputJson);
        }
    }
}
