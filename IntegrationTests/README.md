# Integration tests

This project runs **end-to-end HTTP tests** against the real **API Docker image** and a **SQL Server** instance, both started with **Docker** via [Testcontainers](https://dotnet.testcontainers.org/). Assertions use **[Verify](https://github.com/VerifyTests/Verify)** snapshot files so responses stay stable and reviewable.

---

## What runs where


| Piece           | Where it runs                                                                                |
| --------------- | -------------------------------------------------------------------------------------------- |
| **Test runner** | Your machine / CI (normal `dotnet test` process)                                             |
| **SQL Server**  | Docker container (`Testcontainers.MsSql`), on a dedicated Docker network                     |
| **API**         | Docker container built from `API/Dockerfile`, same network, talks to SQL by hostname `mssql` |
| **HTTP calls**  | `HttpClient` from the test process to `http://127.0.0.1:<mapped-port>` (published API port)  |


  The API is **not** hosted in-process with `WebApplicationFactory` here; it is the same containerized app you would deploy, with `ASPNETCORE_ENVIRONMENT=Testing` and configuration from `appsettings.Testing.json` merged with environment variables (notably `ConnectionStrings__DefaultConnection`).

---

## Lifecycle: one shared fixture per run

Tests that need the stack use xUnit’s **collection fixture** so **all integration tests share one** `TestFixture`:

- `[CollectionDefinition("Integration")]` + `ICollectionFixture<TestFixture>` in `Infrastructure/IntegrationCollectionDefinition.cs`
- Test classes are marked `[Collection("Integration")]` (e.g. `AccountTests`, `UsersTests`)

That avoids starting **two** SQL containers, **two** API containers, or **two** `docker build`s when different test classes run in parallel.

### `TestFixture.InitializeAsync()` (high level)

1. **Docker network** – created so SQL and API can resolve each other by name.
2. `**SqlServerFixture`** – starts MS SQL on that network with alias `**mssql**`, creates database `**TestDb**`, runs **EF migrations** from the test host using the **mapped** SQL connection string (localhost + random port).
3. **API image** – `DockerCli.BuildApiImageAsync` runs `docker build` in the `API` folder (tag `myblog-api:integration-test`). This uses the CLI instead of Testcontainers’ Dockerfile stream API for reliability.
4. **API container** – started on the same network, env `ConnectionStrings__DefaultConnection` set to the **internal** connection string (`Server=mssql,1433;...`).
5. **Wait** – HTTP wait until `GET /` returns success on port **8080** inside the container.
6. **Base URL** – `HttpClient` base address is `http://127.0.0.1:<hostMappedPort>/`.

### `TestFixture.DisposeAsync()`

Disposes API container, then SQL container, then the network (nested `try/finally` so cleanup still runs if one step fails).

---

## Project layout (important folders)


| Path                                 | Role                                                               |
| ------------------------------------ | ------------------------------------------------------------------ |
| `Infrastructure/TestFixture.cs`      | Orchestrates network + SQL + API + `CreateApi()`                   |
| `Infrastructure/SqlServerFixture.cs` | MS SQL Testcontainer, DB creation, host-side migrations            |
| `Infrastructure/DockerCli.cs`        | `docker build` for the API image                                   |
| `Infrastructure/RepositoryPaths.cs`  | Finds repo root via `MyBlog.sln` (walks up from test `bin` output) |
| `Infrastructure/VerifySetup.cs`      | Module initializer: snapshot paths, scrubbers, ignored members     |
| `Clients/`                           | Thin HTTP wrappers: `TestApi`, `AccountsClient`, `UsersClient`     |
| `Builders/`                          | Fluent request DTO builders for tests                              |
| `Helpers/`                           | Auth helpers, `HttpSnapshotSanitizer`                              |
| `Tests/Users/`                       | `AccountTests`, `UsersTests`, `VerifyExtensions`                   |
| `Snapshots/<TestClassName>/`         | `*.verified.txt` golden files; `*.received.txt` on mismatch        |


---

## Calling the API from tests

- `**fixture.CreateApi()`** – new `HttpClient` + `TestApi` per test (avoids leaking `Authorization` or cookies between tests).
- `**fixture.CreateApi(handleCookies: true)**` – same, but with a cookie container (e.g. refresh-token flows).

`TestApi` exposes `Accounts`, `Users`, and raw `Http` for login/token helpers.

---

## Verify snapshots

### Where files live

`VerifySetup` sets:

- Directory: `IntegrationTests/Snapshots/{TestClassName}/`
- File name pattern: `{TestClassName}.{TestMethodName}.verified.txt`

### Global scrubbing / ignores

Configured in `Infrastructure/VerifySetup.cs`:

- **Guids and datetimes** normalized for stable diffs.
- **Sensitive / noisy members** ignored (`token`, `passwordHash`, `correlationId`, etc.).
- **Lines** scrubbed for `Authorization: Bearer …` and `Set-Cookie` refresh tokens.

### Why `HttpSnapshotSanitizer` exists

The API runs behind real **Kestrel** in Docker, so `HttpResponseMessage` snapshots differ from a typical in-memory test server (extra headers like `Date` / `Server`, chunked encoding, host/port in `RequestUri`). Before `Verifier.Verify` runs, helpers in `Helpers/HttpSnapshotSanitizer.cs`:

- Remove volatile headers where the API allows it.
- Normalize `RequestUri` to `http://localhost` without a random port.
- Replace outbound request content with `**StringContent`** so request content headers match stable snapshots.

`VerifyExtensions` in `Tests/Users/VerifyExtensions.cs` wires this in:

- `**Verify()**` – full `HttpResponseMessage` (after sanitizer).
- `**VerifyResponseOnly()**` – status, filtered headers, body (for JSON bodies that are already scrubbed or stable).
- `**VerifyResponseMetaOnly()**` – status + headers only (when bodies contain dynamic user-specific strings).

`UsersTests.GetUsers_after_registering_users_snapshot_contains_sorted_list` builds a snapshot that **lists users** (email, userName, role): it registers two accounts, calls `GET /Users` as admin, then uses `Helpers/UserListSnapshot.cs` to parse the JSON and filter to **admin + `getusers_snapshot_*` rows** so the verified file stays stable while all tests share one database.

### Updating snapshots after intentional API changes

When a test fails, Verify writes `*.received.txt` next to the `*.verified.txt`. Review the diff, then either copy/rename the received file to verified or use your preferred Verify workflow (IDE diff tool, `dotnet verify`, etc.).

---

## Prerequisites

- **Docker** running (Testcontainers + `docker build`).
- **First run** can be slow (base images, `docker build`, SQL pull). Later runs use layer/cache.

---

## Running tests

From the solution root:

```bash
dotnet test IntegrationTests/IntegrationTests.csproj
```

---

## Route placeholders in tests

Some tests use a **fixed GUID** in the URL (e.g. `UsersRoutePlaceholderId`) when the test only cares about status/auth, not the specific id. Verify’s inline GUID scrubber does **not** rewrite GUIDs embedded inside `RequestUri`, so a stable literal keeps snapshots deterministic without custom URI scrubbers.

---

## Further reading in code

- `API/Program.cs` – `Testing` environment: SQL Server uses migrations; seed/admin behavior uses `appsettings.Testing.json`.
- `API/Dockerfile` – image the tests build and run.

