using LLama;
using LLama.Abstractions;

namespace ThisPresentationDoesNotExist;

public sealed class DisposableStatelessExecutor(LLamaWeights weights, IContextParams @params, ILogger? logger = null)
    : StatelessExecutor(weights, @params, logger), IDisposable
{
    private readonly LLamaWeights _weights = weights;

    public void Dispose()
    {
        _weights.Dispose();
    }
}