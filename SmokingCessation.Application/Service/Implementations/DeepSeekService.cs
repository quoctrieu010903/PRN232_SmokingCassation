using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SmokingCessation.Application.Service.Interfaces;

namespace SmokingCessation.Application.Service.Implementations
{
    public class DeepSeekService : IDeepSeekService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl = "https://api.deepseek.com/v1/chat/completions"; // Example endpoint

        public DeepSeekService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["DeepSeek:ApiKey"];
        }

        public async Task<string> GenerateAdviceAsync(string prompt)
        {
            var requestBody = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful coach for quitting smoking." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 512
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseContent);
            var advice = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content").GetString();
            return advice ?? "No advice generated.";
        }

        public async Task<string> GenerateDailyAdviceAsync(string reason, DateTimeOffset startDate, DateTimeOffset targetDate, DateTimeOffset today)
        {
            var prompt = $@"A user is trying to quit smoking.
- Reason: {reason}
- Start date: {startDate:yyyy-MM-dd}
- Target quit date: {targetDate:yyyy-MM-dd}
Today is {today:yyyy-MM-dd}.
Please provide a motivational, practical, and supportive daily advice to help the user stay on track with their quit plan. The advice should be relevant for today and encourage the user to keep going.";

            return await GenerateAdviceAsync(prompt);
        }
    }
} 