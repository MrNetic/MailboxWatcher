using Microsoft.AspNetCore.Mvc;
using MailboxWatcher.Modules.MailboxEvents.Application.ReceiveShellyEvent;
using MailboxWatcher.Modules.MailboxEvents.Presentation.Contracts;
using Microsoft.AspNetCore.RateLimiting;

namespace MailboxWatcher.Modules.MailboxEvents.Presentation
{
    [ApiController]
    [Route("api/mailboxevents")]
    public sealed class MailboxEventsController : ControllerBase
    {
        [HttpPost("shelly")]
        [EnableRateLimiting("shelly")]
        public async Task<ActionResult<ReceiveShellyEventResponse>> ReceiveShellyEvent(
            [FromForm] ReceiveShellyEventRequest request,
            [FromServices] ReceiveShellyEventHandler handler,
            CancellationToken cancellationToken)
        {
            var command = new ReceiveShellyEventCommand(
                request.DeviceId,
                request.Fcd2Hex,request.Timestamp);

            var result = await handler.Handle(command, cancellationToken);

            var response = new ReceiveShellyEventResponse(
                result.EventId,
                result.Status);

            return Ok(response);
        }
    }
}
