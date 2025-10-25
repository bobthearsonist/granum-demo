# granum-demo

A demo project for Granum Interviews.

## Architecture Documentation

I used some tooling to generate a handful of (incomplete) UML diagrams from my initial sketches.

- **[Entity Relationship Diagram](entity-diagram.md)** - Database schema and entity relationships
- **[Class Diagram](class-diagram.md)** - Object-oriented class structure and design patterns
- **[Sequence Diagram](sequence-diagram.md)** - Workflow and actor interactions

- [Initial System Sketch](initial-system-sketch.png) - Original system concept diagram
- [Initial Sequence Sketch](initial-sequence-sketch.png) - Original workflow sketch

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
