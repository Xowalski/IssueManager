using IssueManager.BLL.Contracts;
using System.Text.Json;
using System.Text;

namespace IssueManager.BLL.Services
{
    public class GitHubIssueService : IIssueService
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private readonly string _baseUrl;

        public GitHubIssueService(HttpClient httpClient, string token, string baseUrl)
        {
            _httpClient = httpClient;
            _token = token;
            _baseUrl = baseUrl;

            if (string.IsNullOrEmpty(_token) || string.IsNullOrEmpty(_baseUrl))
            {
                throw new InvalidOperationException("GitHub configuration is missing or incomplete.");
            }
        }

        public async Task AddNewIssueAsync(string owner, string repository, string title, string description)
        {
            var url = $"{_baseUrl}/repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(repository)}/issues";
            var newIssueData = new { title, body = description };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(newIssueData), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_token}");
            request.Headers.Add("User-Agent", "HttpClient-Demo");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task CloseIssueAsync(string owner, string repository, int issueId)
        {
            var url = $"{_baseUrl}/repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(repository)}/issues/{issueId}";
            var payload = new { state = "closed" };

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_token}");
            request.Headers.Add("User-Agent", "HttpClient-Demo");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task EditIssueAsync(string owner, string repository, int issueId, string newTitle, string newDescription)
        {
            var url = $"{_baseUrl}/repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(repository)}/issues/{issueId}";
            var issueEditData = new { title = newTitle, body = newDescription };

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(issueEditData), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_token}");
            request.Headers.Add("User-Agent", "HttpClient-Demo");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}