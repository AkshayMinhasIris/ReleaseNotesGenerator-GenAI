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
            try
            {
                var pullRequests = await _gitHubClient.PullRequest.GetAllForRepository(owner, repo);
                return Ok(pullRequests.Select(pr => new { pr.Title, pr.Body, pr.State }));
            }
            catch(Exception ex) 
            {
                throw;
            }
        }
        //TOdo remove comments
        //Get Github token
        //check PR form github
        //UI page to make list of PR
        //select PR list and send to Open AI
    }
}
