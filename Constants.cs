using ReleaseNotesGenerator.Controllers;

namespace ReleaseNotesGenerator
{
    public static class Constants
    {
        public const string GPTModelName = "gpt-4o-mini";
        public const string ProductHeaderValue = "ReleaseNotesGenerator-GenAI";
        public static readonly string ReleaseNotesPrompt = $@"
                                                        You are an expert in creating concise, professional release notes.
                                                        Summarize the following Pull Request into a structured JSON format suitable for both clients and internal teams.
                                                        **Input:**  
                                                        PR Title: {0}  
                                                        PR Description: {1}  
                                                        **Output Format (JSON):**  
                                                        {{
                                                            ""Features"": ""<Feature description>"",
                                                            ""Details"": ""<Key points such as functionality, usage, or improvement areas>"",
                                                            ""Impact"": ""<How this benefits users or resolves an issue>""
                                                        }}";

        //TODO: need to get it from UI
        public const string Owner = "AkshayMinhasIris";
        public const string Repo = "ReleaseNotesGenerator-GenAI";
    }
}
