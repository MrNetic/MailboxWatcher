namespace MailboxWatcher.Modules.MailboxEvents.Presentation.Contracts
{
    public sealed record ReceiveShellyEventRequest(
        string DeviceId,
        string Fcd2Hex,
        DateTime? Timestamp
    );
}
