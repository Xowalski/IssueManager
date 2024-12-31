using IssueManager.BLL.Contracts;
using IssueManager.BLL.Models;
using IssueManager.BLL.Services;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace IssueManager.BLL.Factories
{
    public static class IssueServiceFactory
    {
        public static IIssueService GetService(string platform, HttpClient httpClient, PlatformSettings platformSettings)
        {
            return platform.ToLower() switch
            {
                "github" => new GitHubIssueService(httpClient, platformSettings.GitHub.Token, platformSettings.GitHub.BaseUrl),
                "gitlab" => new GitLabIssueService(httpClient, platformSettings.GitLab.Token, platformSettings.GitLab.BaseUrl),
                _ => throw new ArgumentException($"Unsupported platform: {platform}")
            };
        }
    }
}
