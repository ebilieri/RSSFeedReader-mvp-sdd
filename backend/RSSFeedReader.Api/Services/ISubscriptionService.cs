using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Services;

public interface ISubscriptionService
{
    IReadOnlyList<SubscriptionItem> GetAll();
    SubscriptionItem Add(string url);
}
