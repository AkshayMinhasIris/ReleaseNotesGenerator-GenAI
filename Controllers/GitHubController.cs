using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Octokit;

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
                Credentials = new Credentials("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")
            };
        }

        [HttpGet("pull-requests")]
        public async Task<IActionResult>GetPullRequests(string owner, string repo)
        {
            var pullRequests = await _gitHubClient.PullRequest.GetAllForRepository(owner, repo);
            return Ok(pullRequests.Select(pr=>new {pr.Title,pr.Body,pr.State}));
        }
        //TOdo remove comments
        //Get Github token
        //check PR form github
        //UI page to make list of PR
        //select PR list and send to Open AI
    }
}
