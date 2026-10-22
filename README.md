# WorkforceIntegrationPOC

This repository is a .NET 8 proof-of-concept for a workforce scheduling solution that combines:

- workforce integration from an external data source
- local persistence with EF Core
- AI-assisted analysis for staffing recommendations

This is a generic workforce scheduling demo. It is not connected to any real company, customer, or production workforce system.

## Purpose

The project demonstrates a clean architecture for:

1. ingesting workforce-related data from a simulated external API
2. transforming and storing it in a local relational database
3. reviewing employees, shifts, and assignments in a simple UI
4. generating a textual mock AI analysis to suggest staffing coverage and gaps

The design is intentionally generic so the same pattern can later be adapted to Azure OpenAI, an HRIS feed, or a workforce planning API.

## Architecture overview

The solution is structured into three layers:

- Web project (`WorkforceIntegrationPOC.Web`)
  - Blazor Server UI
  - Minimal API endpoints
  - HTTP-based interaction with the application services

- Domain project (`WorkforceIntegrationPOC.Domain`)
  - business entities (`Employee`, `Shift`, `Assignment`)
  - interfaces (`IIntegrationService`, `IAnalyticsService`)

- Infrastructure project (`WorkforceIntegrationPOC.Infrastructure`)
  - EF Core DbContext and SQLite database
  - integration service that simulates fetching data from an external system
  - analytics service that reads the data and produces a summary

## How it works

### Integration layer

`IntegrationService` simulates a downstream workforce provider by returning a list of employees and shifts. It then adds the records to the SQLite database and creates matching assignments based on required skills.

This keeps the integration logic isolated from the Web project and preserves the clean layering:

- Web -> Domain -> Infrastructure

### Analytics layer

`AnalyticsService` reads the data from the database, checks for unfilled shifts, and builds a textual summary describing staffing coverage, skills, and overall schedule stability.

The implementation is intentionally structured as a mock AI analysis service so an Azure OpenAI integration can be added later with minimal impact on the UI and data flow.

## Running the project

1. Open a terminal in the repository root.
2. Restore packages:

   ```bash
   dotnet restore
   ```

3. Run the solution:

   ```bash
   dotnet run --project src/WorkforceIntegrationPOC.Web/WorkforceIntegrationPOC.Web.csproj
   ```

4. Open the local URL shown by ASP.NET Core (typically `https://localhost:XXXX`).

## Included sample data

The app seeds a few sample employees, shifts, and assignments on first run. This ensures the solution is usable without any external setup.

## Notes

- The default database is SQLite.
- The code is designed so it can later be swapped to SQL Server or Azure-hosted services with minimal code changes.
- The app does not interact with a real company or external workforce system; it is a demonstration only.
