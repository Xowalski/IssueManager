using Microsoft.AspNetCore.Mvc;
using IssueManager.BLL.Factories;
using IssueManager.BLL.Models;
using Microsoft.Extensions.Options;
using IssueManager.BLL;

namespace IssueManager.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<PlatformSettings> _platformSettings;

        public IssuesController(IHttpClientFactory httpClientFactory, IOptions<PlatformSettings> platformSettings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _platformSettings = platformSettings;
        }

        [HttpPost("{platform}/{owner}/{repository}")]
        public async Task<IActionResult> AddIssue(string platform, string owner, string repository, [FromBody] IssueRequest request)
        {
            var service = IssueServiceFactory.GetService(platform, _httpClient, _platformSettings.Value);
            await service.AddNewIssueAsync(owner, repository, request.Title, request.Description);
            return Ok("Issue added successfully.");
        }

        [HttpPut("{platform}/{owner}/{repository}/{issueId}")]
        public async Task<IActionResult> EditIssue(string platform, string owner, string repository, int issueId, [FromBody] IssueRequest request)
        {
            var service = IssueServiceFactory.GetService(platform, _httpClient, _platformSettings.Value);
            await service.EditIssueAsync(owner, repository, issueId, request.Title, request.Description);
            return Ok("Issue edited successfully.");
        }

        [HttpPatch("{platform}/{owner}/{repository}/{issueId}/close/")]
        public async Task<IActionResult> CloseIssue(string platform, string owner, string repository, int issueId)
        {
            var service = IssueServiceFactory.GetService(platform, _httpClient, _platformSettings.Value);
            await service.CloseIssueAsync(owner, repository, issueId);
            return Ok("Issue closed successfully.");
        }
    }
}