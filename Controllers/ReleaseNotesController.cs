using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ReleaseNotesGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReleaseNotesController(AzureOpenAIClient openAIClient, ILogger<ReleaseNotesController> logger) : ControllerBase
    {
        private readonly AzureOpenAIClient _openAIClient = openAIClient;
        private readonly ILogger<ReleaseNotesController> _logger = logger;

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReleaseNotes([FromBody] List<PRInput> inputs)
        {
            if (inputs == null || !inputs.Any())
            {
                _logger.LogWarning("Input list is empty.");
                return BadRequest("Input list cannot be empty.");
            }


            ChatClient chatClient = _openAIClient.GetChatClient(Constants.GPTModelName);
            var options = new ChatCompletionOptions
            {
                Temperature = 0.7f,
                MaxOutputTokenCount = 800,
                TopP = 0.95f,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            };
            var releaseNotes = new List<object>();

            foreach (var input in inputs)
            {
                string prompt = ServiceExtension.ReleaseNotesPrompt(input.Title, input.Description);
                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(prompt),
                };
                try
                {
                    _logger.LogInformation("Generating release notes for PR: {Title}", input.Title);
                    ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);

                    // Extract the text content
                    var contentText = completion.Content?.FirstOrDefault()?.Text;

                    // Logging for debugging
                    _logger.LogDebug("Generated Content: {ContentText}", contentText);

                    // Extract JSON block if the response includes additional text
                    var jsonMatch = Regex.Match(contentText ?? "", @"\{[\s\S]*\}").Value;

                    if (!string.IsNullOrEmpty(jsonMatch))
                    {
                        var jsonContent = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonMatch);
                        releaseNotes.Add(jsonContent);
                    }
                    else
                    {
                        releaseNotes.Add(new
                        {
                            FeaturesUpdate = "N/A",
                            Details = "N/A",
                            Impact = $"Failed to generate structured notes for PR '{input.Title}'."
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating release notes for PR: {Title}", input.Title);
                    releaseNotes.Add(new
                    {
                        FeaturesUpdate = "N/A",
                        Details = "N/A",
                        Impact = $"Error: {ex.Message}"
                    });
                }
            }
            return Ok(releaseNotes);
        }
    }
}
