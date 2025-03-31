using Azure;
using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using Octokit;
using OpenAI.Chat;
using System.Reflection;
using System.Text.Json;
using Azure.Identity;
using System.Text.RegularExpressions;

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

        //[HttpPost("generate")]
        //public async Task<IActionResult> GenerateReleaseNotes([FromBody] PRInput input)
        //{
        //    string prompt = $@"You are an expert in creating concise, professional release notes. Summarize the following
        //                        Pull request into a structure format suitable for both clients and internal teams.
        //                    Input :
        //                    PR Title :{input.Title}
        //                    PR Description : {input.Description}

        //                    Output Format :
        //                    - Features/Update: Brief description of what was added, changed, or fixed.
        //                    - Details: Key Ponts, such as functionality, usuage, or improvements areas.
        //                    - Impact: How this benefits users or resolves an issue.";

        //    ChatClient chatClient = _openAIClient.GetChatClient("gpt-4o-mini");           
        //    var messages = new List<ChatMessage>
        //                    {
        //                     new SystemChatMessage(prompt),

        //                    };
        //    var options = new ChatCompletionOptions
        //    {
        //        Temperature = (float)0.7,
        //        MaxOutputTokenCount = 800,

        //        TopP = (float)0.95,
        //        FrequencyPenalty = (float)0,
        //        PresencePenalty = (float)0
        //    };

        //    string data = null;

        //    try
        //    {              
        //        ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);                
        //        data = JsonSerializer.Serialize(completion.Content, new JsonSerializerOptions() { WriteIndented = true });
        //    }
        //    catch (Exception ex)
        //    {
        //       throw new Exception(ex.Message);
        //    }
        //    return Ok(data); 
        //}

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReleaseNotes([FromBody] List<PRInput> inputs)
        {
            if (inputs == null || !inputs.Any())
                return BadRequest("Input list cannot be empty.");

            ChatClient chatClient = _openAIClient.GetChatClient("gpt-4o-mini");
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
                string prompt = $@"
       You are an expert in creating concise, professional release notes.
       Summarize the following Pull Request into a structured JSON format suitable for both clients and internal teams.
       **Input:**  
       PR Title: {input.Title}  
       PR Description: {input.Description}  
       **Output Format (JSON):**  
       {{
           ""Features"": ""<Feature description>"",
           ""Details"": ""<Key points such as functionality, usage, or improvement areas>"",
           ""Impact"": ""<How this benefits users or resolves an issue>""
       }}";

                var messages = new List<ChatMessage> { new SystemChatMessage(prompt) };
                try
                {
                    ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);

                    // Extract the text content
                    var contentText = completion.Content?.FirstOrDefault()?.Text;

                    // Logging for debugging
                    Console.WriteLine($"Generated Content: {contentText}");

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
