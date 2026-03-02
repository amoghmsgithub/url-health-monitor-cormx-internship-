using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UrlHealthMonitor.Models;

namespace UrlHealthMonitor.Services
{
    public class RecoveryClientService
    {
        private readonly HttpClient _httpClient;

        private const string ApiKey =
            "tejasrapikeyforhealthcheckapi@8689973corm";

        public RecoveryClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RecoveryAckResponse?> TriggerRecoveryAsync(MonitoredUrl endpoint)
        {
            if (endpoint == null ||
                string.IsNullOrWhiteSpace(endpoint.RecoveryWebsiteUrl) ||
                string.IsNullOrWhiteSpace(endpoint.AppPoolName) ||
                !endpoint.RecoveryAction.HasValue)
            {
                return null;
            }

            var payload = new
            {
                action = endpoint.RecoveryAction.Value,
                target = endpoint.AppPoolName
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                endpoint.RecoveryWebsiteUrl
            );

            request.Headers.Add("X-Recovery-Key", ApiKey);
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);

            // ❌ DO NOT deserialize webhook.site response
            if (!response.IsSuccessStatusCode)
            {
                return new RecoveryAckResponse
                {
                    Ack = false,
                    Message = "Recovery endpoint returned non-success status"
                };
            }

            // ✅ ACK-1 accepted
            return new RecoveryAckResponse
            {
                Ack = true,
                RequestId = Guid.NewGuid().ToString(),
                Message = "Recovery request accepted"
            };
        }
    }

    public class RecoveryAckResponse
    {
        public bool Ack { get; set; }
        public string? RequestId { get; set; }
        public string? Message { get; set; }
    }
}
