using IssueManager.BLL.Contracts;
using System.Text.Json;
using System.Text;

namespace IssueManager.BLL.Services
{
    public class GitLabIssueService : IIssueService
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private readonly string _baseUrl;

        public GitLabIssueService(HttpClient httpClient, string token, string baseUrl)
        {
            _httpClient = httpClient;
            _token = token;
            _baseUrl = baseUrl;

            if (string.IsNullOrEmpty(_token) || string.IsNullOrEmpty(_baseUrl))
            {
                throw new InvalidOperationException("GitLab configuration is missing or incomplete.");
            }
        }

        public async Task AddNewIssueAsync(string owner, string repository, string title, string description)
        {
            var projectId = $"{owner}/{repository}";
            var url = $"{_baseUrl}/projects/{Uri.EscapeDataString(projectId)}/issues";
            var newIssueData = new { title, description };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(newIssueData), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_token}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task CloseIssueAsync(string owner, string repository, int issueId)
        {
            var projectId = $"{owner}/{repository}";
            var url = $"{_baseUrl}/projects/{Uri.EscapeDataString(projectId)}/issues/{issueId}";
            var payload = new { state_event = "close" };

            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_token}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task EditIssueAsync(string owner, string repository, int issueId, string newTitle, string newDescription)
        {
            var projectId = $"{owner}/{repository}";
            var url = $"{_baseUrl}/projects/{Uri.EscapeDataString(projectId)}/issues/{issueId}";
            var issueEditData = new { title = newTitle, description = newDescription };

            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(issueEditData), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_token}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}