using System.Reflection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace ThisPresentationDoesNotExist.Results;

public class OllamaResult(Kernel kernel, KernelFunction function, string request)
    : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "text/plain";
        await foreach (var response in kernel.InvokeStreamingAsync(function, new()
                       {
                           { "request", request }
                       }))
        {
            await httpContext.Response.WriteAsync(response.ToString());
        }
    }
}