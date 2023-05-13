using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebMagic
{
    internal class ChatGPTAPI
    {
        private string apiKey = "";
        private readonly HttpClient _httpClient;

        public ChatGPTAPI()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            apiKey = configuration.GetValue<string>("gptApiKey");
            var timeout = TimeSpan.FromMinutes(3);

            _httpClient = new HttpClient()
            {
                Timeout = timeout
            };
        }

        internal string GetResponse(string v)
        {
            var response = this.getChatGPTResponse(v);
            response.Wait();
            return response.Result;
        }
       
        public async Task<string> getChatGPTResponse(string prompt)
        {
            var timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            Console.WriteLine("Running prompt " + timestamp);
            var model = "gpt-3.5-turbo-0301"; // Use the latest model here
            var messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            };

            var payload = new
            {
                model = model,
                messages = messages
            };

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var stringContent = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", stringContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get chat completion. Status code: {response.StatusCode}, Content: {responseContent}");
                }
                dynamic responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                Console.WriteLine("Success " + timestamp);
                // Console.WriteLine(responseObject.choices[0].message.content.ToString());
                return responseObject.choices[0].message.content.ToString();
                // var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
                // Console.WriteLine(result.FirstChoice);
                // return result.FirstChoice;
            }
            catch (Exception e)
            {
                Console.WriteLine(@"=============================================================================================================
                Reinstating the API call due to exception: ", e.Message);
                // print timestamp in IST
                timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                Console.WriteLine(timestamp);
                CommandProcessor.LogJsonParsingError(e, e.Message, prompt);
                // wait
                System.Threading.Thread.Sleep(10000);
                return GetResponse(prompt);
            }

        }
        private  async Task<string> getChatGPTResponse_old(string prompt)
        {
            var api = new OpenAIClient(new OpenAIAuthentication(apiKey));
            string response = string.Empty;
            var chatPrompts = new List<ChatPrompt>
            {
                new ChatPrompt("system", "You are a helpful assistant."),
                new ChatPrompt("user", prompt)
            };
            var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo);

            var timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            Console.WriteLine("Running prompt " + timestamp);
            
            try
            {
                var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
                Console.WriteLine(result.FirstChoice);
                return result.FirstChoice;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"=============================================================================================================
                Reinstating the API call due to exception: ", ex.Message);
                // print timestamp in IST
                timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                Console.WriteLine(timestamp);
                // wait
                System.Threading.Thread.Sleep(10000);
                return GetResponse(prompt);
            }
            return response;
        }
    }
}