using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace WebMagic
{
    public class OpenAIConversation
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAIConversation(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.openai.com/v1/completions"),
                Headers =
                {
                        {"Authorization", $"Bearer {_apiKey}"}
                },
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("prompt", prompt),
                    new KeyValuePair<string, string>("model", "text-davinci-003"),
                    new KeyValuePair<string, string>("max_tokens", "100"),
                    new KeyValuePair<string, string>("temperature", "0.7"),
                    new KeyValuePair<string, string>("top_p", "1"),
                    new KeyValuePair<string, string>("frequency_penalty", "0"),
                    new KeyValuePair<string, string>("presence_penalty", "0"),
                })
            };

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            // Parse the response to extract the generated text
            // Parse the JSON data to a JsonDocument object
            JsonDocument doc = JsonDocument.Parse(responseJson);

            // Get the root element of the document
            JsonElement root = doc.RootElement;
            root.TryGetProperty("choices", out JsonElement choices);
            if (choices.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement choice in choices.EnumerateArray())
                {
                    choice.TryGetProperty("text", out JsonElement text);
                    return text.ToString();
                }
            }

            // Parse the response to extract the generated text
            // The response from the API is in the format of a JSON object
            // We need to extract the "choices" array and get the "text" property of the first item in the array
            // var jsonObject = JObject.Parse(responseJson);
            // var choices = jsonObject.Value<JArray>("choices");
            // var firstChoice = choices[0];
            // var text = firstChoice.Value<string>("text");

            

            return "text";
        }
    }
}
