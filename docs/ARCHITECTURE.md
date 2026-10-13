# Architecture & design decisions

## Context

A manufacturing team needs equipment visibility and incident tracking. This demo focuses on a tractable vertical slice rather than pretending to be an MES replacement.

## Components

1. **Angular UI:** renders overview KPIs, equipment list, telemetry chart and incident workflow. Typed service calls backend endpoints. Refresh every 15 s.
2. **ASP.NET Core API:** Minimal API endpoints, application validation and incident state machine. The simulator is a hosted background service.
3. **EF Core + SQLite:** relational persistence with machine-to-readings and machine-to-incidents one-to-many relationships.
4. **Docker Compose:** separates nginx frontend and API, persists SQLite in a Docker volume.

## Why these choices?

- **ASP.NET Core and Angular:** deliberate match for common enterprise full-stack workflows and the referenced role's explicit technology stack.
- **SQLite:** no separate server needed for onboarding. Future deployment could adopt SQL Server/PostgreSQL with migrations and relational integration tests.
- **Simulated telemetry:** enables reproducible demos and avoids implying access to real factory equipment.
- **REST rather than SignalR:** simple to understand and test. Add server-push only when polling is an actual bottleneck.
- **Minimal API:** small vertical slice with explicit endpoints; split into services/controllers if the surface grows.

## Data model

`Machine (1) -> (N) TelemetryReading`; `Machine (1) -> (N) Incident`.

Readings are periodic *interval samples*, with `UnitsProduced` counting output **during each sample interval**. The overview's `unitsToday` is a sum of readings since midnight **UTC**, not per-shift or per-local-site production. The demo creates 15-minute historical samples and 15-second live ones; units are fabricated solely for UI purposes.

`availabilityPercent` is the **currently running share of machines**, not time-weighted availability or OEE. Do not present it as plant performance KPI without a proper production data model.

## Incident state diagram

`Open → Acknowledged → Resolved`, or `Open → Resolved`. Resolved is terminal. The rules are unit tested.

## Security / non-goals

The demo has no authentication and allows incident writes on local development endpoints. No production deployment or exposure to the public internet is approved. Production requirements include authentication/authorization, audit logging, request rate limits, migrations, health monitoring, secret management, telemetry retention and realistic machine connectivity standards.

## Future connectivity integration

A later optional integration might accept simulated OPC UA device readings via an adapter. This must be clearly separated from real PLC hardware and is not required for the Software Engineer learning track.
