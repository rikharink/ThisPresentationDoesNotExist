using OllamaSharp;

namespace ThisPresentationDoesNotExist.Repositories;

public interface IChatContextRepository
{ 
    void StoreContext(string key, ConversationContext? context);
    ConversationContext? GetContext(string key);
    void Reset();
}