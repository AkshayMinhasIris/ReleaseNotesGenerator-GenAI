using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using OpenAI.Chat;
using OpenAI;
using System.Text.Json;

namespace ReleaseNotesGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly GitHubClient _gitHubClient;
        public GitHubController()
        {
            _gitHubClient = new GitHubClient(new ProductHeaderValue("ReleaseNotesGenerator-GenAI"))
            {
                //TODO: get from URL documents
                Credentials = new Credentials("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")
            };
        }

        [HttpGet("pull-requests")]
        public async Task<IActionResult>GetPullRequests(string owner, string repo)
        {
            try
            {
                var pullRequests = await _gitHubClient.PullRequest.GetAllForRepository(owner, repo, new PullRequestRequest { State = ItemStateFilter.All});
                return Ok(pullRequests.Select(pr => new { pr.Title, pr.Body, pr.State }));
            }
            catch(Exception ex) 
            {
                throw;
            }
        }        
    }
}
