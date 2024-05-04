using OllamaSharp;

namespace ThisPresentationDoesNotExist.Repositories.Implementations;

public class MemoryChatContextRepository : IChatContextRepository
{
    private readonly Dictionary<string, ConversationContext?> _contexts = new();
    
    public void StoreContext(string key, ConversationContext? context)
    {
        _contexts[key] = context;
    }

    public ConversationContext? GetContext(string key)
    {
        return _contexts.TryGetValue(key, out var context) ? context : null;
    }
    
    public void Reset() => _contexts.Clear();
}