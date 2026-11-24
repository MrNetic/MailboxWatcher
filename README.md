# 📬 MailboxWatcher

MailboxWatcher turns movement of a physical mailbox flap into an email notification, using Shelly hardware and an ASP.NET Core API.

## Why this project exists

This project started with a missed letter. I did not check my mailbox in time, and ended up having to queue at a public service office to sort things out. That inconvenience sparked a simple idea: what if my mailbox could tell me when something happened?

Shelly hardware can detect whether a door is open or closed. I adapted that idea to the mailbox flap, so that movement can trigger an email and remind me to check for post.

### Hardware used for the build

Sensor: [Shelly BLU Door/Window](https://www.shelly.com/blogs/documentation/blu-door-window)

<img width="424" height="333" alt="image" src="https://github.com/user-attachments/assets/302f247b-330c-4de0-a6b4-01aa86816f20" />



Gateway: [Shelly Bluetooth gateway](https://www.shelly.com/products/shelly-blu-gateway)

<img width="424" height="333" alt="image" src="https://github.com/user-attachments/assets/c914452f-4114-4372-b906-ca884f5d63e1" />


My actual physical mailbox itself \*g\*
<img width="1086" height="1448" alt="MyPhysicalMailbox" src="https://github.com/user-attachments/assets/1adb7020-9a13-4605-9dd7-da9a18bbfaa4" />

## ⚙️ How it works

1. Shelly hardware detects a change in the mailbox flap's state.
2. The Shelly setup forwards an event to the API.
3. The API processes the event and sends an email through MailerSend.

Flap movement is a reminder to check the mailbox; it does not guarantee that a letter has been delivered.

## Current implementation

- ASP.NET Core API targeting .NET 10.
- A `POST /api/mailboxevents/shelly` endpoint that receives form data from the Shelly setup.
- The Shelly endpoint accepts one request per fixed one-minute window across all callers, per application instance. Excess requests receive HTTP 429 and a `Retry-After` header, without being queued. Invalid requests also consume the allowance. This limit resets when the application restarts and does not replace authentication or event deduplication.
- Event processing with structured logging and a generated event ID.
- Email notifications through MailerSend.
- Code organised into mailbox event and notification modules.

The current source does not yet include explicit Azure Key Vault or Application Insights integration. Those services are part of the project's learning and integration goals.

## Shelly Bluetooth gateway script

Copy [`shelly/mailbox-gateway.js`](shelly/mailbox-gateway.js) into a script on the Shelly gateway, set `targetAddr` to the sensor's Bluetooth address, and start the script. It runs a passive scan, converts the sensor's `fcd2` service data to hex, and logs the result of each HTTP POST.

The script sends JSON with `deviceId` and `fcd2Hex` to the configured Azure `/api/MailboxWatcher/shelly` endpoint. To use the API in this repository, point `endpoint` at `/api/mailboxevents/shelly` on your API host and send the two fields as `application/x-www-form-urlencoded`.

## Shelly request rate limiting

The endpoint uses ASP.NET Core's built-in rate limiter to allow one request per fixed one-minute window, shared across all callers within an application instance. Requests above the limit return HTTP 429 with a `Retry-After` header and are not queued.

The rationale is to reduce bursts of email notifications caused by repeated device requests or several flap movements close together, and to limit calls to the email provider. A shared limit keeps the behaviour simple for the current mailbox use case; using a caller-supplied device ID as the partition key would allow callers to bypass the limit by changing that ID.

This is an in-memory request limit: restarting the application resets it, and multiple instances have independent limits. Requests that reach the limiter consume the allowance even if validation or email delivery later fails. Fixed windows can allow two accepted requests close together across a window boundary, so this is not a strict 60-second interval between notifications. Authentication, event deduplication and reliable delivery remain separate planned improvements.

## You got Mail! (real physical one :))

<img width="395" height="211" alt="image" src="https://github.com/user-attachments/assets/70061166-7088-42cb-9e73-f4ab6dcde3f6" />


## 🛠️ Planned improvements ( as time/ priorities permits)

This is still a work in progress. Next steps include:

- **SQL Server persistence:** store mailbox events, device details and notification outcomes to build an event history.
- **Web UI:** provide a simple interface to browse that history and see recent mailbox activity.
- **Azure Key Vault integration:** manage sensitive configuration such as email provider credentials through secrets.
- **Azure Application Insights integration:** trace events through the API, investigate failures and monitor notification delivery.
- **Event interpretation:** decode the Shelly payload into meaningful open/closed states and decide which changes should trigger an email.
- **Duplicate handling:** avoid repeated notifications for duplicate events or several flap movements close together.
- **Reliable notifications:** add background processing and retries for temporary email provider failures.
- **Endpoint protection and validation:** authenticate incoming device events and validate their payloads.

The aim is to keep the solution practical: a small reminder that helps prevent another missed letter, with room to learn and improve along the way.
