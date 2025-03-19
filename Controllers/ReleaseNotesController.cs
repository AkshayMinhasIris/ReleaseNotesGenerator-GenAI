using Azure;
using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using Octokit;
using OpenAI.Chat;
using System.Reflection;
using System.Text.Json;
using Azure.Identity;

namespace ReleaseNotesGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReleaseNotesController : ControllerBase
    {
        private readonly AzureOpenAIClient _openAIClient;
        public ReleaseNotesController()
        {
           var endpoint = new Uri("https://aksha-m8ddshn8-eastus2.cognitiveservices.azure.com/");
           var apiKey = "AGVaGSzPrXZ72jqQ7P17sSFPDHB2sHdmn9OB07olPKyMLt5i9MnVJQQJ99BCACHYHv6XJ3w3AAAAACOG2xNw";
           _openAIClient = new(endpoint, new AzureKeyCredential(apiKey));

        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReleaseNotes([FromBody] PRInput input)
        {
            string prompt = $@"You are an expert in creating concise, professional release notes. Summarize the following
                                Pull request into a structure format suitable for both clients and internal teams.
                            Input :
                            PR Title :{input.Title}
                            PR Description : {input.Description}

                            Output Format :
                            - **Features/Update**: Brief description of what was added, changed, or fixed.
                            - **Details**: Key Ponts, such as functionality, usuage, or improvements areas.
                            - **Impact**: How this benefits users or resolves an issue.";
           
            ChatClient chatClient = _openAIClient.GetChatClient("gpt-4o-mini");           
            var messages = new List<ChatMessage>
                            {
                             new SystemChatMessage(prompt),
              
                            };
            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.7,
                MaxOutputTokenCount = 800,

                TopP = (float)0.95,
                FrequencyPenalty = (float)0,
                PresencePenalty = (float)0
            };

            string data = null;

            try
            {              
                ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);                
                data = JsonSerializer.Serialize(completion.Content, new JsonSerializerOptions() { WriteIndented = true });
            }
            catch (Exception ex)
            {
               throw new Exception(ex.Message);
            }
            return Ok(data); //Todo
        }
    }
}
