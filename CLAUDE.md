# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Purpose

Customer-facing demo that illustrates how **settings versioning** is solved using the REST API versioning mechanism. The concept: as settings evolve, each version of the settings schema is exposed as a distinct API version, so consumers can pin to a version and migrate on their own schedule.

This goal is **not yet implemented** — the codebase is an early-stage scaffold: the layered structure is in place, but most types (`SettingsService`, `SettingsDomain`, `SettingsEntity`, `SettingsDto`, `ISettingsService`) are empty stubs, and `Program.cs` is still the default ASP.NET template (`/weatherforecast`).

### Planned GitFlow alignment

The API versioning demo is intended to mirror a **GitFlow** branching model, so the version control flow itself reinforces the demo narrative:

- Each settings schema version corresponds to a long-lived branch (e.g. `release/v1`, `release/v2`).
- Breaking changes to the settings model are introduced on a new version branch rather than in-place, keeping older API versions stable on their branches.
- `main` tracks the latest stable version; `develop` is where the next version is assembled.

Keep this mapping in mind when adding new settings versions or deciding where to land a change.

## Commands

Run from the `DemoApiVersioning/` directory (where `DemoApiVersioning.slnx` lives):

```bash
dotnet build                              # build the whole solution
dotnet run --project Api                  # run the API (http profile → http://localhost:5184)
dotnet run --project Api --launch-profile https   # https → https://localhost:7008
```

There is no test project yet. When one is added, run a single test with:
```bash
dotnet test --filter "FullyQualifiedName~<TestName>"
```

OpenAPI is mapped only in Development (`/openapi`). `Api/Api.http` contains sample requests.

## Architecture

Three-project layered solution targeting **.NET 10** (`Nullable` and `ImplicitUsings` enabled everywhere). Solution uses the newer XML `.slnx` format.

- **`Api`** (`Microsoft.NET.Sdk.Web`) — HTTP entry point. Minimal API, not MVC. DTOs live in `Api/Dtos`.
- **`Core`** (`Microsoft.NET.Sdk`) — business logic. Services under `Core/Services/<Feature>/`, domain models under `Core/Domains/`. DI wiring lives in `Core/DiCompositor.cs`.
- **`Infrastructure.Database`** (`Microsoft.NET.Sdk`) — persistence. Entities under `Infrastructure.Database/Entities/`.

**Intended dependency flow** is `Api → Core → Infrastructure.Database`, with a type per layer for each concept (e.g. `SettingsDto` → `SettingsDomain` → `SettingsEntity`). Note: the `.csproj` files do **not** yet declare `ProjectReference`s between projects — wire these up before code in one layer can use another.

## Conventions

- **Endpoints are static extension methods on `WebApplication`**, one static class per feature. See `Api/Controllers/SettingsController.cs`: `app.GetSettings()` calls `app.MapGet("/settings", ...).WithName("GetSettings")` and returns `app` for chaining. Register new endpoints by calling these extensions in `Program.cs`.
- **DI registration is centralized** in `Core/DiCompositor.cs` via the generic `AddCore<T>(this T serviceCollection)` extension. Add new service registrations there (`AddScoped<IFoo, Foo>()`), and call `builder.Services.AddCore()` from `Program.cs`.
- Service implementations are `internal sealed`; their interfaces are `public` (so `Api` can depend on the abstraction, not the implementation).
