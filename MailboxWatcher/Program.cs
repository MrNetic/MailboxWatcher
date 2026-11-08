using MailboxWatcher;
using MailboxWatcher.Modules.MailboxEvents.Application.ReceiveShellyEvent;
using System.Globalization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("shelly", _ =>
        RateLimitPartition.GetFixedWindowLimiter("shelly", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
    options.OnRejected = (context, _) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                Math.Ceiling(retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        return ValueTask.CompletedTask;
    };
});

//Add ReceiveShellyEventHandler to the dependency injection container
//builder.Services.AddScoped<ReceiveShellyEventHandler>();
builder.Services.AddSingleton<ReceiveShellyEventHandler>();

//Add MailerSendService to the dependency injection container
builder.Services.AddSingleton<MailerSendService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
