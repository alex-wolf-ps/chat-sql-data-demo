using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Text.Json;
using YourOwnData;

namespace StoryCreator.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string UserPrompt { get; set; } = string.Empty;
        public List<List<string>> RowData { get; set; }
        public string Summary { get; set; }
        public string Query { get; set; }
        public string Error { get; set; }

        public void OnPost()
        {
            RunQuery(UserPrompt);
        }

        public void RunQuery(string userPrompt)
        {
            // Configure OpenAI client
            string openAIEndpoint = "YOUR_ENDPOINT";
            string openAIKey = "YOUR_KEY";
            string openAIDeploymentName = "YOUR_MODE_NAME";

            OpenAIClient client = new(new Uri(openAIEndpoint), new AzureKeyCredential(openAIKey));

            // Use the SchemaLoader project to export your db schema and then paste the schema in the placeholder below
            var systemMessage = @"Your are a helpful, cheerful database assistant. 
            Use the following database schema when creating your answers:

            YOUR_DATABASE_SCHEMA

            Include column name headers in the query results.

            Always provide your answer in the JSON format below:
            
            { ""summary"": ""your-summary"", ""query"":  ""your-query"" }
            
            Output ONLY JSON.
            In the preceding JSON response, substitute ""your-query"" with Microsoft SQL Server Query to retrieve the requested data.
            In the preceding JSON response, substitute ""your-summary"" with a summary of the query.
            Always include all columns in the table.
            If the resulting query is non-executable, replace ""your-query"" with NA, but still substitute ""your-query"" with a summary of the query.
            Do not use MySQL syntax.
            Always limit the SQL Query to 100 rows."; // change this based on your db

            // Set up the AI chat query/completion
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages = {
                    new ChatRequestSystemMessage(systemMessage),
                    new ChatRequestUserMessage(userPrompt)
                },
                DeploymentName = openAIDeploymentName
            };

            // Send request to Azure OpenAI model
            try
            {
                ChatCompletions chatCompletionsResponse = client.GetChatCompletions(chatCompletionsOptions);
            
                var response = JsonSerializer
                    .Deserialize<AIQuery>(chatCompletionsResponse.Choices[0].Message.Content
                    .Replace("```json", "").Replace("```", ""));
                
                Summary = response.summary;
                Query = response.query;
                RowData = DataService.GetDataTable(response.query);
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
        }
    }

    public class AIQuery
    {
        public string summary { get; set; }
        public string query { get; set; }
    }
}