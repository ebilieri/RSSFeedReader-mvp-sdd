using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Services;

public class InMemorySubscriptionService : ISubscriptionService
{
    private readonly List<SubscriptionItem> _subscriptions = [];
    private readonly object _lock = new();

    public IReadOnlyList<SubscriptionItem> GetAll()
    {
        lock (_lock) return _subscriptions.AsReadOnly();
    }

    public SubscriptionItem Add(string url)
    {
        var item = new SubscriptionItem { Url = url.Trim() };
        lock (_lock) _subscriptions.Add(item);
        return item;
    }
}
