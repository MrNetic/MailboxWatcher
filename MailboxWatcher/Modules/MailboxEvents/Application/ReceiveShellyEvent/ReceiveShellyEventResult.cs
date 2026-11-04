namespace MailboxWatcher.Modules.MailboxEvents.Application.ReceiveShellyEvent
{
    public sealed record ReceiveShellyEventResult(
        Guid EventId,
        string Status);
}
