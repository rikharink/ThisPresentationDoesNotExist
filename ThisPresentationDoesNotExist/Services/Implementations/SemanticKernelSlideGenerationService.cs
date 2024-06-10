using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using ThisPresentationDoesNotExist.Results;

namespace ThisPresentationDoesNotExist.Services.Implementations;

public class SemanticKernelSlideGenerationService(
    ILogger<SemanticKernelSlideGenerationService> logger, Kernel kernel) : ISlideGenerationService
{
    private readonly ChatHistory history = new("""
                                               Your role is to create a slide for a presentation.
                                               You are allowed to think before answering, put your thought process between <thinking></thinking> tags
                                               You use markdown syntax to format your response
                                               Read these instructions and the prompt carefully and create a slide that answers the prompt.
                                               You only respond with the slide content in markdown, no pre-ambles or explanations
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

                                               <|start_header_id|>user<|end_header_id|>
                                               Cool facts about llamas
                                               <|eot_id|><|start_header_id|>assistant<|end_header_id|>
                                               <thinking> I think about some cool facts about LLama's. Then I summarize these facts in max 5 bullet points.
                                               I use emoji to emphasize my point where applicable and summarize the content in a nice title. To finish I return the nicely formatted markdown slide</thinking>
                                               # Llamas: The Amazing Animals
                                               - ** Fiber Frenzy**: Llamas are used to produce clothing, hats, and scarves.
                                               - ** Agile Adventures**: Llamas are great at hiking and trekking.
                                               - ** Social Skills**: Llamas are social animals and like to be around other llamas.
                                               - ** Intelligent Insight **: Llamas are smart and can learn new things quickly and can even learn to open gates.
                                               - ** Communicative Creatures **: Llamas communicate with each other through humming, ear movements, snorting and spitting.
                                               <|eot_id|>
                                               <|eot_id|>
                                               <|start_header_id|>user<|end_header_id|>
                                               {{request}}
                                               <|eot_id|>
                                               <|start_header_id|>assistant<|end_header_id|>
                                               """);
    
    public Task<IResult> GenerateSlide(string prompt)
    {
        logger.LogInformation("Prompt: {Prompt}", prompt);
        
        IResult result = new SemanticKernelResult(kernel, history, prompt); 
        return Task.FromResult(result);
    }
}