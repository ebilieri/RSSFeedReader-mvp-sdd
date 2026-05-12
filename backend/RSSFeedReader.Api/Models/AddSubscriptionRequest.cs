using System.ComponentModel.DataAnnotations;

namespace RSSFeedReader.Api.Models;

public class AddSubscriptionRequest
{
    [Required]
    public required string Url { get; set; }
}
