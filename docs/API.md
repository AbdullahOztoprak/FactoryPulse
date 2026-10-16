# API reference

Base URL in local development: `http://localhost:5080`.

Responses use camelCase property names and string-valued enums.

## Get overview

```http
GET /api/overview
```

Includes `machineCount`, `runningCount`, `unitsToday`, `openIncidents`, and `dataSource: "SIMULATED"`.

## Get machines

```http
GET /api/machines
```

Returns `id`, `code`, `name`, `line`, `status`, latest temperature and vibration metrics for each machine. Take an `id` from this endpoint for the next calls.

## Get machine detail

```http
GET /api/machines/{id}
```

Returns one machine with its latest reading and open incident count. A missing machine returns **404 Not Found**.

## Get readings

```http
GET /api/machines/{machineId}/readings?limit=30
```

Readings are ordered oldest-to-newest. `limit` is capped at 100. A missing machine returns 404.

## Create incident

```http
POST /api/incidents
Content-Type: application/json

{
  "machineId": "<copy-real-id-from-GET-machines>",
  "title": "Temperature above operating range",
  "description": "Synthetic condition observed during demo testing.",
  "severity": "High"
}
```

Response: **201 Created** with the new incident ID and status. Malformed data/unknown machine: **400 Bad Request**.

## Update incident

```http
PATCH /api/incidents/{incidentId}/status
Content-Type: application/json

{ "status": "Acknowledged" }
```

Allowed transitions: `Open → Acknowledged → Resolved`, or `Open → Resolved`. Other transitions return **400**. Missing incident returns **404**.

## Read incidents

```http
GET /api/incidents
```

Returns the newest 50 records, including machine code and status.

## Development OpenAPI

When running the API with `ASPNETCORE_ENVIRONMENT=Development`, navigate to `/openapi/v1.json`.
