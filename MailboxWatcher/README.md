# 📬 MailboxWatcher

MailboxWatcher turns movement of a physical mailbox flap into an email notification, using Shelly hardware and an ASP.NET Core API.

## Why this project exists

This project started with a missed letter. I did not check my mailbox in time, and ended up having to queue at a public service office to sort things out. That inconvenience sparked a simple idea: what if my mailbox could tell me when something happened?

Shelly hardware can detect whether a door is open or closed. I adapted that idea to the mailbox flap, so that movement can trigger an email and remind me to check for post.

It also became an opportunity to refresh some technical knowledge around **Azure Key Vault secrets** and **Azure Application Insights**, while building something useful for an everyday problem.

## ⚙️ How it works

1. Shelly hardware detects a change in the mailbox flap's state.
2. The Shelly setup forwards an event to the API.
3. The API processes the event and sends an email through MailerSend.

Flap movement is a reminder to check the mailbox; it does not guarantee that a letter has been delivered.

## Current implementation

- ASP.NET Core API targeting .NET 10.
- A `POST /api/mailboxevents/shelly` endpoint that receives form data from the Shelly setup.
- Event processing with structured logging and a generated event ID.
- Email notifications through MailerSend.
- Code organised into mailbox event and notification modules.

The current source does not yet include explicit Azure Key Vault or Application Insights integration. Those services are part of the project's learning and integration goals.

## 🛠️ Planned improvements

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
