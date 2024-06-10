using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ThisPresentationDoesNotExist.Results;

public class SemanticKernelResult(Kernel kernel, ChatHistory history, string userMessage)
    : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            Temperature = 0.8
        };
        
        history.AddUserMessage(userMessage);
        
        httpContext.Response.ContentType = "text/plain";
        
        var result = chatCompletionService.GetStreamingChatMessageContentsAsync(history,
            executionSettings: executionSettings, kernel: kernel);

        var fullMessageBuilder = new StringBuilder();
        await foreach (var content in result)
        {
            if (content.Content == null)
            {
                continue;
            }
            
            await httpContext.Response.WriteAsync(content.Content);
            fullMessageBuilder.Append(content.Content);
        }
        
        history.AddAssistantMessage(fullMessageBuilder.ToString());
    }
}