using Azure.AI.OpenAI;
using Azure;
using Octokit;
using ReleaseNotesGenerator.Controllers;

namespace ReleaseNotesGenerator
{
    public static class ServiceExtension
    {
        public static void AddAzureOpenAIClient(this IServiceCollection services, IConfiguration configuration)
        {
            var endpoint = new Uri(configuration["AzureOpenAI:Endpoint"]);
            var apiKey = configuration["AzureOpenAI:ApiKey"];
            var openAIClient = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey));
            services.AddSingleton(openAIClient);
        }

        public static void AddGitHubClient(this IServiceCollection services, IConfiguration configuration)
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue("ReleaseNotesGenerator-GenAI"))
            {
                Credentials = new Credentials(configuration["GitHubToken"])
            };
            services.AddSingleton(gitHubClient);
        }
        
        public static string ReleaseNotesPrompt(string title, string description)
        {
            return $@" You are an expert in creating concise, professional release notes.
                       Summarize the following Pull Request into a structured JSON format suitable for both clients and internal teams.
                       **Input:**  
                            PR Title: {title}  
                            PR Description: {description}  
                       **Output Format (JSON):**  
                        {{
                            ""Features"": ""<Feature description>"",
                            ""Details"": ""<Key points such as functionality, usage, or improvement areas>"",
                            ""Impact"": ""<How this benefits users or resolves an issue>""
                        }}";
        }
    }
}
