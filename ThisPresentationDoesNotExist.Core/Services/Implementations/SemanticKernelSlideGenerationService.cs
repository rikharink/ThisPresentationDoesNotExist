using ThisPresentationDoesNotExist.Services;

namespace ThisPresentationDoesNotExist.Core.Services.Implementations;

public class SemanticKernelSlideGenerationService(
    ILogger<SemanticKernelSlideGenerationService> logger,
    Kernel kernel) : ISlideGenerationService
{
    private const string SystemPrompt = """
                                        Your role is to create a slide for a presentation.
                                        You use markdown syntax to format your response
                                        Read these instructions and the prompt carefully and create a slide that answers the prompt.
                                        You only respond with the slide content in markdown, no pre-ambles or explanations
                                        If I give you further instructions don't acknowledge the new instructions, dont reply, just follow them and create the slide
                                        
                                        You only answer with a title and the bullet points to summarize your point
                                        The only exception is when asked for an example than you answer in title and code fence block
                                        
                                        One title per slide
                                        A maximum of five bullet points per slide
                                        Limit nesting of bullet points, unless absolutely necessary to make your point
                                        
                                        You use a maximum of 25 words
                                        Your tone should be humorous but professional, you may use emojis to emphasize your points.
                                        
                                        You keep the slide simple and easy to understand, the talking will explain, the slide is only for reference
                                        Use best practices for slide content
                                        Proof-read the slide for spelling and grammar and make sure the content fits the prompt

                                        So for example:
                                        
                                        <|start_header_id|>assistant<|end_header_id|>
                                        <thinking> I think about some cool facts about LLama's. Then I summarize these facts in max 5 bullet points.
                                        I use emoji to emphasize my point where applicable and summarize the content in a nice title. To finish I return the nicely formatted markdown slide</thinking>
                                        # Llamas: The Amazing Animals 🦙
                                        - ** Fiber Frenzy**: Llamas are used to produce clothing, hats, and scarves. 🧵
                                        - ** Agile Adventures**: Llamas are great at hiking and trekking. 🚶
                                        - ** Social Skills**: Llamas are social animals and like to be around other llamas. 🗣️
                                        - ** Intelligent Insight **: Llamas are smart and can learn new things quickly and can even learn to open gates. 🚪
                                        - ** Communicative Creatures **: Llamas communicate with each other through humming, ear movements, snorting and spitting. 💬
                                        <|eot_id|>
                                        <|start_header_id|>user<|end_header_id|>
                                        Cool facts about Llamas
                                        <|eot_id|>
                                        <|start_header_id|>assistant<|end_header_id|>
                                       
                                        and an example for a user asking for an example:
                                         
                                        <|start_header_id|>assistant<|end_header_id|>
                                        # Shellscript AI: The Scripting Sidekick 🤖
                                        ```text
                                        Your role is to create a shell script for a specific task.
                                        I'll provide you with the task and you'll write the script.
                                        If anything is unclear, ask for clarification.
                                        If I give you feedback on your script only respond with the changes you made, not the whole script.
                                        Please explain the thought  process of your script in short before giving the code.
                                        ```
                                        <|eot_id|>
                                        <|start_header_id|>user<|end_header_id|>
                                        Give me an example for a system prompt for an AI that helps you write a shellscript.
                                        <|eot_id|>
                                        <|start_header_id|>assistant<|end_header_id|>
                                        """;

    private ChatHistory _history = new(SystemPrompt);

    public async Task<string> GenerateSlideNonStreaming(string prompt)
    {
        logger.LogInformation("Prompt: {Prompt}", prompt);
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            Temperature = 0.8
        };

        _history.AddUserMessage(prompt);
        var result =
            await chatCompletionService.GetChatMessageContentsAsync(_history, executionSettings: executionSettings,
                kernel: kernel);
        return result[0].Content ?? "";
    }

    public IAsyncEnumerable<string> GenerateSlide(string prompt)
    {
        logger.LogInformation("Prompt: {Prompt}", prompt);
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            Temperature = 0.8
        };

        _history.AddUserMessage(prompt);
        var result = chatCompletionService.GetStreamingChatMessageContentsAsync(_history,
            executionSettings: executionSettings, kernel: kernel);

        return result
            .Where(r => r.Content != null)
            .Select(r =>
            {
                var content = r.Content!;
                return content;
            });
    }

    public void AddResponseToHistory(string response)
    {
        _history.AddAssistantMessage(response);
    }

    public void ResetHistory()
    {
        _history = new ChatHistory(SystemPrompt);
    }
}