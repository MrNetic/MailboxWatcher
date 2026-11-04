namespace MailboxWatcher.Modules.MailboxEvents.Application.ReceiveShellyEvent
{
    public sealed record ReceiveShellyEventCommand(
        string DeviceId,
        string Fcd2Hex,
    DateTime? Timestamp);
}
