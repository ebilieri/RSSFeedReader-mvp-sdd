using Microsoft.AspNetCore.Mvc;
using RSSFeedReader.Api.Models;
using RSSFeedReader.Api.Services;

namespace RSSFeedReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _service;

    public SubscriptionsController(ISubscriptionService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult Post([FromBody] AddSubscriptionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var item = _service.Add(request.Url);
        return CreatedAtAction(nameof(Get), item);
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_service.GetAll());
    }
}
