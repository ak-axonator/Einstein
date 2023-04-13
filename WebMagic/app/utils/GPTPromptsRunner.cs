using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace WebMagic
{
    public class GPTPromptsRunner
    {
        public static void Run(string prompt, string outputFilePath)
        {
            ChatGPTAPI  ai = new ChatGPTAPI();
            Console.WriteLine("Running prompt: "+ prompt);
            string result = ai.GetResponse(prompt);
            if (string.IsNullOrEmpty(result))
            {
                CommandProcessor.LogJsonParsingError(new Exception("Empty response from GPT"), "Empty response from GPT", prompt);
                return;
            }
            // Console.WriteLine("ChatGPT Response: " + result);
            // Write output JSON to file
            File.WriteAllText(outputFilePath, result);
            Console.WriteLine($"prompt response stored in {outputFilePath}");
            System.Threading.Thread.Sleep(1000);
        }
    }
}
