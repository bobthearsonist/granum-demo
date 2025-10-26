# granum-demo

A demo project for Granum Interviews.

## Architecture Documentation

I used some tooling to generate a handful of (incomplete) UML diagrams from my initial sketches.

- **[Entity Relationship Diagram](entity-diagram.md)** - Database schema and entity relationships
- **[Class Diagram](class-diagram.md)** - Object-oriented class structure and design patterns
- **[Sequence Diagram](sequence-diagram.md)** - Workflow and actor interactions

- [Initial System Sketch](initial-system-sketch.png) - Original system concept diagram
- [Initial Sequence Sketch](initial-sequence-sketch.png) - Original workflow sketch

## Backend

run the server (this is just until its containerized)
```bash
cd apps/backend
dotnet run --project Granum.Api
# API available at https://localhost:7001
```

run the tests
```bash
cd apps/backend
# Run all tests
dotnet test
# Run only unit tests
dotnet test Granum.Tests/
# Run only integration tests
dotnet test Granum.IntegrationTests/
# Run with verbose output
dotnet test -v d
```

**`Granum.Tests`** - Unit tests for business logic
- Service layer tests with mocked dependencies
- Validator tests for FluentValidation rules  
- Repository tests with in-memory database

**`Granum.IntegrationTests`** - Controller-level integration tests
- Uses `WebApplicationFactory` to test HTTP endpoints
- NUnit test framework with FluentAssertions
- In-memory database for test isolation

I looked at Gherkin-based frameworks (SpecFlow) because ive used them in the past but decided it was not appropriate for a project.
- The current API logic is currently simple
- would add unnecessary overhead and complexity when stakeholders don't require business-readable test scenarios at this stage, ie no-one else is writing these
- Basic controller-level tests with clear naming provide sufficient coverage with less maintenance burden

I may revisit this methodology for system level tests once the UI is implemented.

## toolchain

created with some scaffold tools

```sh
cd $ROOT/apps/frontend && npm create vite@latest . -- --template react-ts && npm install
cd $ROOT/apps/backend
dotnet new webapi -n Granum.Api -o Granum.Api
dotnet new nunit -n Granum.Tests -o Granum.Tests && dotnet add $ROOT/apps/backend/Granum.Tests/Granum.Tests.csproj reference $ROOT/apps/backend/Granum.Api/Granum.Api.csproj
dotnet new sln -n Granum && dotnet sln Granum.sln add Granum.Api/Granum.Api.csproj Granum.Tests/Granum.Tests.csproj
```

## commit conventions

This project follows [Conventional Commits](https://www.conventionalcommits.org/) with these prefixes:
