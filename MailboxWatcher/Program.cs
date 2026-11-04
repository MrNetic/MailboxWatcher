using MailboxWatcher;
using MailboxWatcher.Modules.MailboxEvents.Application.ReceiveShellyEvent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
