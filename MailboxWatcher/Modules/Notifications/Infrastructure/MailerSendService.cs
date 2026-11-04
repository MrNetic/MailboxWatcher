using System.Text;
using System.Text.Json;

public class MailerSendService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public MailerSendService(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public async Task SendEmailAsync(string deviceId, string fcd2Hex)
    {
        var apiToken = _configuration["MailerSend:ApiToken"];
        var fromEmail = _configuration["MailerSend:FromEmail"];
        var fromName = _configuration["MailerSend:FromName"];
        var toEmails = _configuration.GetSection("MailerSend:ToEmails").Get<string[]>();

        var payload = new
        {
            from = new
            {
                email = fromEmail,
                name = fromName
            },
            to = toEmails.Select(e => new { email = e }),
            subject = "📬 Mailbox event detected",
            text = $"Device: {deviceId}\nPayload: {fcd2Hex}",
            html = $"<strong>Mailbox event</strong><br/>Device: {deviceId}<br/>Payload: {fcd2Hex}"
        };

        var json = JsonSerializer.Serialize(payload);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mailersend.com/v1/email");
        request.Headers.Add("Authorization", $"Bearer {apiToken}");
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"MailerSend error: {response.StatusCode} - {body}");
        }
    }
}