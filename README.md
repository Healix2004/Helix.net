# Helix Healthcare API

Helix is a modular healthcare backend built with ASP.NET Core and C#. It provides REST APIs and real-time notifications for patient, doctor, appointment, prescription, laboratory, terminology, and administrative workflows.

> **Status:** Active development
>
> This repository contains backend services only. Review the API documentation and deployment notes before connecting a client application or deploying to a shared environment.

## Features

- JWT-based authentication and role-aware authorization
- Doctor appointment dashboards and patient clinical timelines
- Patient and provider workflows
- Prescription management
- Laboratory and terminology integrations, including LOINC and FHIR
- SNOMED CT-backed clinical data and seeded reference datasets
- Swagger/OpenAPI documentation for interactive API testing
- SignalR notifications through the `/notification` hub
- Entity Framework Core database initialization, migrations, and seeders
- Docker and Docker Compose support
- Layered solution structure for maintainability

## Technology stack

- **C# / .NET 10**
- **ASP.NET Core Web API**
- **Entity Framework Core 10**
- **Swagger / OpenAPI** via Swashbuckle
- **FHIR** via HL7 FHIR libraries
- **SignalR** for real-time notifications
- **Docker** for containerized development and deployment

## Repository structure

| Project or directory | Purpose |
| --- | --- |
| `HelixAPI/` | ASP.NET Core host, controllers, hubs, middleware, configuration, and API endpoints |
| `Helix.Core/` | Core DTOs, features, base types, and shared application contracts |
| `Helix.Data/` | Domain entities and enumerations |
| `Helix.Infrastructure/` | Persistence, database context, initialization, and infrastructure concerns |
| `Helix.Service/` | Services, repositories, mappings, specifications, and business logic |
| `k8s/` | Kubernetes deployment resources |
| `docker-compose.yml` | Container build and local orchestration configuration |
| `HelixSn.sln` | Visual Studio solution file |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git
- A database supported by the configured Entity Framework Core provider
- Docker Desktop (optional, for containerized development)
- Credentials for any external FHIR, LOINC, SMTP, or other integrations used by your environment

Check the installed SDK:

```bash
dotnet --version
```

## Getting started

### 1. Clone the repository

```bash
git clone https://github.com/Healix2004/Helix.net.git
cd Helix.net
```

### 2. Configure secrets

Do not commit passwords, connection strings, JWT signing keys, SMTP credentials, or external API credentials. Use one of the following for local development:

- ASP.NET Core user secrets
- Environment variables
- A local, untracked configuration file
- A managed secret store in deployed environments

The main configuration sections are:

- `ConnectionStrings:DefaultConnection`
- `JwtSettings`
- `EmailSettings`
- `Fhir`
- `FhirLab`
- `LoincApi`
- `CorsSettings:AllowedOrigins`

For user secrets, run this from the `HelixAPI` directory:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
dotnet user-secrets set "JwtSettings:Secret" "<long-random-signing-key>"
```

Add only the settings required by the integrations enabled in your environment. Never use production credentials for local development.

### 3. Restore, build, and run

```bash
dotnet restore HelixSn.sln
dotnet build HelixSn.sln --configuration Release
dotnet run --project HelixAPI/Helix.API.csproj
```

The application initializes the database and seeds configured reference data during startup. Make sure the configured database is reachable before running the API.

### 4. Open the API documentation

When the application is running, Swagger UI is available at:

```text
https://localhost:7193/swagger
```

The exact port can vary by launch profile or environment. Check `HelixAPI/Properties/launchSettings.json` or the console output if this URL is not available.

## Docker

Build and start the API with Docker Compose:

```bash
docker compose up --build
```

Stop the services with:

```bash
docker compose down
```

Provide configuration through environment variables or an approved secret-management solution rather than embedding secrets in the image or compose files.

## API authentication

Most protected endpoints require a bearer token:

```http
Authorization: Bearer <jwt>
```

Some doctor-to-patient workflows additionally require a consent token:

```http
X-Consent-Token: <consent-jwt>
```

Use Swagger or the API reference documents in this repository for endpoint-specific roles, request payloads, and response formats. Do not use sample patient data or credentials in production.

## Documentation

- [Helix API REST reference](helix-api-docs.md)
- [Doctor appointment API](DOCTOR_APPOINTMENT_API.md)
- [Frontend integration guide](FRONTEND_INTEGRATION_GUIDE.md)
- [Frontend endpoint and role guide](FRONTEND_ENDPOINT_ROLE_GUIDE.md)
- [Lab, radiology, and consent API guide](FRONTEND_API_LAB_RADIOLOGY_CONSENT.md)
- [LOINC integration guide](LOINC_INTEGRATION_GUIDE.md)
- [LOINC quick reference](LOINC_QUICK_REFERENCE.md)
- [Deployment guide](DEPLOYMENT_GUIDE.md)
- [Kubernetes resources](k8s/)

## Development workflow

Run the standard validation commands before opening a pull request:

```bash
dotnet restore HelixSn.sln
dotnet build HelixSn.sln --configuration Release
```

When changing API behavior:

1. Update the relevant controller, service, DTO, and mapping layers.
2. Verify authentication and authorization requirements.
3. Update Swagger/XML documentation and the appropriate Markdown API guide.
4. Test successful and failure responses, including `400`, `401`, `403`, `404`, and `500` cases where applicable.
5. Confirm that logs do not expose credentials or protected health information.

## Security and privacy

This application handles sensitive healthcare data. Before deployment:

- Rotate any credentials that may have been exposed in repository history or configuration files.
- Store all secrets outside source control.
- Use HTTPS and a strong, randomly generated JWT signing key.
- Configure CORS to allow only trusted client origins.
- Apply least-privilege database and external-service credentials.
- Avoid logging tokens, passwords, connection strings, or patient health information.
- Review authorization and consent-token behavior for every patient-data endpoint.
- Apply database migrations and seed data deliberately in controlled environments.
- Perform an independent security and compliance review before production use.

## Contributing

1. Create a focused branch from the default branch.
2. Make the smallest change that solves the problem.
3. Update tests and documentation where behavior changes.
4. Run the build and relevant checks locally.
5. Open a pull request describing the change, configuration impact, and validation performed.

## License

No license is currently declared in this repository. Contact the repository maintainers before using, distributing, or deploying this project outside its intended context.
