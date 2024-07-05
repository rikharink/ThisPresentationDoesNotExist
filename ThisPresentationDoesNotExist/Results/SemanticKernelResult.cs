using System.Text;
using ThisPresentationDoesNotExist.Services;

namespace ThisPresentationDoesNotExist.Results;

public class SemanticKernelResult(IAsyncEnumerable<string> responseChunks, ISlideGenerationService slideGenerationService)
    : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "text/plain";
        var responseBuilder = new StringBuilder();
        await foreach (var content in responseChunks)
        {
            responseBuilder.Append(content);
            await httpContext.Response.WriteAsync(content);
        }
        slideGenerationService.AddResponseToHistory(responseBuilder.ToString());
    }
}