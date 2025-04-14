using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TelegramServiceHandler.Promts;

namespace TelegramServiceHandler.ChatGPT
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string? _modelName;

        public OpenAiService(string modelName = "llama3")
        {
            _modelName = modelName;
        }

        public async Task<string> GetAIResponseAsync(string userMessage)
        {
            var requestBody = new
            {
                model = _modelName,
                messages = new[]
                {
                    //new { role = "system", content = "You are a friendly Telegram bot. Answer simply and briefly." },
                    new { role = "system", content = PromtBase.mainPromt},
                    new { role = "user", content = userMessage }
                },
                stream = false
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:11434/api/chat", content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                dynamic? parsed = JsonConvert.DeserializeObject(responseText);
                return parsed?.message?.content?.ToString() ?? "[AI dont have an answer]";
            }
            else
            {
                return $"[Error AI]: {response.StatusCode} - {responseText}";
            }
        }

        public (string? userText, string? jsonText) SplitAiResponse(string aiResponse)
        {
            var match = Regex.Match(aiResponse, @"<json>(.*?)</json>", RegexOptions.Singleline);

            if (match.Success)
            {
                string json = match.Groups[1].Value.Trim();
                string text = aiResponse.Replace(match.Value, "").Trim(); 
                return (text, json);
            }

            return (aiResponse, null); 
        }

    }
}
