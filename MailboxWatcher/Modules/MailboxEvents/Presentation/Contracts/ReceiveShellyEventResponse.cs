namespace MailboxWatcher.Modules.MailboxEvents.Presentation.Contracts
{
    public sealed record ReceiveShellyEventResponse(
        Guid Id,
        string Status
    );
}