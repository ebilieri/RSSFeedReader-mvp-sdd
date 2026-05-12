using RSSFeedReader.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5213")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddSingleton<ISubscriptionService, InMemorySubscriptionService>();

var app = builder.Build();

app.UseCors("AllowBlazorFrontend");

app.MapControllers();

app.Run();
