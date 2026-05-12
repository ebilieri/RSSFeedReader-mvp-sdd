using System.Net.Http.Json;
using RSSFeedReader.UI.Models;

namespace RSSFeedReader.UI.Services;

public class SubscriptionApiService
{
    private readonly HttpClient _http;

    public SubscriptionApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<SubscriptionItemDto?> AddAsync(string url)
    {
        var response = await _http.PostAsJsonAsync("subscriptions", new { url });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SubscriptionItemDto>();
    }

    public async Task<List<SubscriptionItemDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<SubscriptionItemDto>>("subscriptions")
               ?? [];
    }
}
