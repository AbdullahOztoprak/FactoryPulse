# Development roadmap

## P0 — Verify the starter (before any public claims)

- [ ] Backend restore/build/test locally
- [ ] Angular dependency install/typecheck/build locally
- [ ] Run the complete app and verify GET/POST/PATCH via UI and curl
- [ ] Verify Docker Compose build and browser behavior
- [ ] Generate and commit `frontend/package-lock.json`; switch installs to `npm ci`
- [ ] Capture authentic application screenshot in `docs/screenshots/`
- [ ] Fix all CI errors and replace unverified badge with an actual GitHub Actions badge

## P1 — Better application design

- [ ] Add machine detail endpoint + filtering/pagination
- [ ] Add incident assignee and timestamps
- [ ] Angular router and feature components
- [ ] API integration tests with test database
- [ ] Real EF Core migrations + migrate on deploy (instead of `EnsureCreated`)
- [ ] Search and filter UI, accessibility checks

## P2 — Professional production-readiness lessons

- [ ] Authentication / authorization (roles: operator, maintenance, admin)
- [ ] Audit trail for incident updates
- [ ] Structured logging and exception mapping
- [ ] Telemetry retention + configurable polling
- [ ] CI coverage and security checks
- [ ] SQL Server / PostgreSQL variant
- [ ] Azure DevOps pipeline example

## P3 — Optional industrial connectivity

- [ ] OPC UA **simulator** integration behind adapter interface
- [ ] Handle device offline / stale data
- [ ] Mock MES data exchange
- [ ] Clearly document that no real machine is connected

## Initial user stories

1. **As an operator**, I can see running/idle/fault machines so I can identify interrupted equipment.
   **Acceptance:** list returns all assets with latest reading; status visible in UI.
2. **As a maintenance engineer**, I can register and acknowledge an issue so we can track responsibility.
   **Acceptance:** create returns 201, transition Open→Acknowledged allowed, invalid transition returns 400.
3. **As a supervisor**, I can view production output and warnings so I can prioritize investigation.
   **Acceptance:** overview contains output since UTC midnight and unresolved incidents.
4. **As a developer**, I can automatically build and test changes so regressions are caught early.
   **Acceptance:** CI has jobs for Angular build/typecheck and .NET build/test.
