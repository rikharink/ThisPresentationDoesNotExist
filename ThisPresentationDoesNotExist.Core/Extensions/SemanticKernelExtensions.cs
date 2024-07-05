using Microsoft.SemanticKernel;
using Serilog;

namespace ThisPresentationDoesNotExist.Extensions;

public static class SemanticKernelExtensions
{
    public static IServiceCollection AddLLama(this IServiceCollection services, LLama settings)
    {

        services.AddSingleton<Kernel>(_ =>
        {
            var customHttpMessageHandler = new CustomHttpMessageHandler();
            customHttpMessageHandler.CustomLlmUrl = settings.ApiUrl;
            var client = new HttpClient(customHttpMessageHandler);
            var builder = Kernel.CreateBuilder();
            builder.Services.AddSerilog();
            builder.AddOpenAIChatCompletion(settings.ModelName, settings.ApiKey, httpClient: client);
            return builder.Build();
        });
        return services;
    }
}

public class CustomHttpMessageHandler : HttpClientHandler
{
    public string CustomLlmUrl { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string[] urls = ["api.openai.com", "openai.azure.com", "localhost"];
        // validate if request.RequestUri is not null and request.RequestUri.Host is in urls
        if (request.RequestUri != null && urls.Contains(request.RequestUri.Host))
        {
            // set request.RequestUri to a new Uri with the LLMUrl and request.RequestUri.PathAndQuery
            request.RequestUri = new Uri($"{CustomLlmUrl}{request.RequestUri.PathAndQuery}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}