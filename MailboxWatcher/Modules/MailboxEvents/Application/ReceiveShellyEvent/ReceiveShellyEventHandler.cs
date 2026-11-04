namespace MailboxWatcher.Modules.MailboxEvents.Application.ReceiveShellyEvent
{
    public sealed class ReceiveShellyEventHandler
    {
        private readonly MailerSendService _mailerSendService;
        private readonly ILogger<ReceiveShellyEventHandler> _logger;

        public ReceiveShellyEventHandler(
            MailerSendService mailerSendService,
            ILogger<ReceiveShellyEventHandler> logger)
        {
            _mailerSendService = mailerSendService;
            _logger = logger;
        }

        public async Task<ReceiveShellyEventResult> Handle(
            ReceiveShellyEventCommand command,
            CancellationToken cancellationToken = default)
        {
            var eventId = Guid.NewGuid();

            _logger.LogInformation(
                "Processing mailbox event {EventId} from device {DeviceId} with payload {Payload}",
                eventId,
                command.DeviceId,
                command.Fcd2Hex);

            await _mailerSendService.SendEmailAsync(
                command.DeviceId,
                command.Fcd2Hex);

            return new ReceiveShellyEventResult(
                eventId,
                "received");
        }
    }
}
