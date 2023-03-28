using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace WebMagic
{
    internal class ChatGPTAPI
    {
        private string apiKey = "";

        public ChatGPTAPI()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            apiKey = configuration.GetValue<string>("gptApiKey");
        }

        internal string GetResponse(string v)
        {
            var response = this.getChatGPTResponse(v);
            response.Wait();
            return response.Result;
        }
       

        private  async Task<string> getChatGPTResponse(string prompt)
        {
            var api = new OpenAIClient(new OpenAIAuthentication(apiKey));
            string response = string.Empty;
            var chatPrompts = new List<ChatPrompt>
            {
                new ChatPrompt("system", "You are a helpful assistant."),
                new ChatPrompt("user", prompt)
            };
            var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

            Console.WriteLine(result.FirstChoice);

            return result.FirstChoice;
        }
    }
}