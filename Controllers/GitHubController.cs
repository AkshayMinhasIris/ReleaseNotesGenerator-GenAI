using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace ReleaseNotesGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController(GitHubClient gitHubClient, ILogger<GitHubController> logger) : ControllerBase
    {
        private readonly GitHubClient _gitHubClient = gitHubClient;
        private readonly ILogger<GitHubController> _logger = logger;

        [HttpGet("pull-requests")]
        public async Task<IActionResult> GetPullRequests(string owner = Constants.Owner, string repo = Constants.Repo)
        {
            try
            {
                _logger.LogInformation("Fetching pull requests for {Owner}/{Repo}", owner, repo);
                var pullRequests = await _gitHubClient.PullRequest.GetAllForRepository(owner, repo, new PullRequestRequest { State = ItemStateFilter.All });
                return Ok(pullRequests.Select(pr => new { pr.Number, pr.Title, pr.Body, pr.State.StringValue, pr.CreatedAt, pr.UpdatedAt, pr.Url, pr.User.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pull requests for {Owner}/{Repo}", owner, repo);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
