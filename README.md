# Authentication and Authorization API

A .NET 10 Web API exploring user registration, password-based sign-in, JWTs, and permission-based authorization with a role/permission data model. The solution uses a layered architecture with API, Application, Domain, and Infrastructure projects.

> **Implementation status:** Authentication and RBAC are not yet fully connected end to end. The project contains JWT configuration and generation, registration/login handlers, permission policies, and role/permission entities. However, the current API pipeline does not call `UseAuthentication()`, generated JWTs do not contain permission claims, and no role/permission assignment or lookup flow is implemented. The current EF Core migration creates only the `Users` table. See [Known gaps](#known-gaps) before relying on protected endpoints.

## What is implemented

- **Registration:** `POST /api/auth/register` validates a name, email, password, and confirmation, hashes the password with BCrypt, and stores the user.
- **Login:** `POST /api/auth` looks up a user by email, verifies the BCrypt hash, and creates a signed JWT response.
- **JWT validation configuration:** The API configures bearer-token validation for issuer, audience, lifetime, and signing key.
- **Permission authorization building blocks:** A dynamic authorization policy provider and handler check for a `permission` claim. `GET /api/users` requests the `user:update` policy.
- **RBAC domain model:** Entities represent users, roles, permissions, user-role assignments, and role-permission assignments.
- **Data access:** Entity Framework Core with SQL Server and a user repository.
- **Application patterns:** MediatR request handlers and a FluentValidation pipeline for registration.

## Architecture

| Project | Responsibility |
| --- | --- |
| `MyApp.API` | ASP.NET Core endpoints, JWT bearer configuration, authorization, OpenAPI, and Swagger |
| `MyApp.Application` | MediatR handlers, DTOs, validation, permission policy provider and handler, and service contracts |
| `MyApp.Domain` | User, role, permission, and association entities |
| `MyApp.Infrastructure` | EF Core `ApplicationDbContext`, SQL Server registration, migrations, and repository implementations |

## API endpoints

### Register a user

`POST /api/auth/register`

```json
{
  "name": "Jane Doe",
  "email": "jane@example.com",
  "password": "StrongPass123",
  "confirmPassword": "StrongPass123"
}
```

Registration validation currently requires a non-empty name (up to 100 characters), a valid email (up to 256 characters), and a password of 8–100 characters containing uppercase, lowercase, and numeric characters. The confirmation must match the password. A successful response contains the new user's ID, name, and email; it does not sign the user in.

### Sign in

`POST /api/auth`

```json
{
  "email": "jane@example.com",
  "password": "StrongPass123"
}
```

A successful response contains `isSuccess`, `message`, and `data` with the user ID, name, email, token, and token validity duration. Login failures currently return a failure response from the handler; the controller responds with HTTP 200 for both successful and unsuccessful login attempts.

The response field is named `tokenValidityMin`, but the current implementation calculates it in seconds.

### List users

`GET /api/users`

This endpoint is annotated with the `user:update` authorization policy. It returns user IDs, names, and email addresses. Permission-based access is not currently operational; see [Known gaps](#known-gaps).

## Configuration

The API reads the SQL Server connection string from `ConnectionStrings:DefaultConnection` and JWT settings from `JwtSettings`.

For local development, set these values using user secrets or environment variables rather than committing secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your SQL Server connection string>" --project MyApp.API
dotnet user-secrets set "JwtSettings:Secret" "<a long random signing key>" --project MyApp.API
dotnet user-secrets set "JwtSettings:Issuer" "<your issuer>" --project MyApp.API
dotnet user-secrets set "JwtSettings:Audience" "<your audience>" --project MyApp.API
```

The equivalent environment variable names are `ConnectionStrings__DefaultConnection`, `JwtSettings__Secret`, `JwtSettings__Issuer`, and `JwtSettings__Audience`. `JwtSettings:ExpirationMinute` controls token lifetime; the checked-in configuration sets it to 60 minutes.

The current checked-in application settings contain a placeholder-like JWT secret and empty issuer/audience values. Configure valid values before testing JWT validation, and use a strong, unique secret outside development.

## Run locally

Prerequisites: .NET 10 SDK and a SQL Server instance accessible to the configured connection string.

From the repository root:

```powershell
dotnet restore
dotnet ef database update --project MyApp.Infrastructure --startup-project MyApp.API
dotnet run --project MyApp.API
```

The launch profiles use `https://localhost:7031` and `http://localhost:5148`. In Development, Swagger UI is available at `/swagger`, and the OpenAPI document is available at `/openapi/v1.json`.

The checked-in initial migration creates only the `Users` table. It does not create the RBAC tables represented by the current `ApplicationDbContext`; a migration that includes the current model is needed before those entities can be persisted.

## Authentication and authorization flow (intended)

The existing login handler follows this sequence:

1. Find the user by email.
2. Verify the submitted password against the stored BCrypt hash.
3. Generate a signed JWT containing the user's ID, name, and email.
4. Return the token and user details in the response.

The permission authorization handler checks whether the authenticated principal has a claim with type `permission` and a value matching the requested policy name (for example, `user:update`). A complete RBAC flow still needs to load a user's roles and permissions, include the resulting permission claims in the token or resolve them during authorization, and ensure bearer authentication runs before authorization.

## Important implementation details

### Password handling

- Registration hashes the submitted password with BCrypt before saving it; the plain-text password is not assigned to the user entity.
- Login verifies the supplied password against the stored hash with BCrypt.
- Registration validation is wired through a MediatR pipeline behavior. The listed name, email, password, and confirmation rules apply to registration; there is no corresponding login validator in the current implementation.
- Registration currently does not check for an existing email before inserting a user. Do not assume duplicate-email prevention is implemented.

### JWT contents and validation

- Tokens are configured with issuer, audience, signing key, and an expiration time from `JwtSettings`.
- The token generator creates user ID, email, and name claims, and signs the token using HMAC-SHA512.
- The generated token currently contains no role or permission claims. Although the response DTO has a `refreshToken` field, refresh-token issuance is not implemented.
- `tokenValidityMin` is currently calculated in seconds, despite its name.
- Token creation currently uses `JwtSecurityToken.ToString()` rather than serializing the token with `JwtSecurityTokenHandler.WriteToken`. Treat the returned token as unverified until this is corrected.

### RBAC model

The intended relationship is **user → role → permission**. `UserRole` represents the user-to-role link, and `RolePermission` links roles with permissions. The policy provider treats a policy name (such as `user:update`) as the required permission name, and the authorization handler checks for a matching `permission` claim.

These are currently building blocks, not a complete working RBAC feature: there are no role/permission assignment endpoints or seeding logic, role-to-permission persistence is not yet fully migrated, and the token/authentication pipeline does not supply permission claims to the handler.

### Example requests

With the API running locally, registration can be tried with:

```powershell
Invoke-RestMethod -Method Post `
  -Uri "https://localhost:7031/api/auth/register" `
  -ContentType "application/json" `
  -Body '{"name":"Jane Doe","email":"jane@example.com","password":"StrongPass123","confirmPassword":"StrongPass123"}'
```

Sign in with:

```powershell
Invoke-RestMethod -Method Post `
  -Uri "https://localhost:7031/api/auth" `
  -ContentType "application/json" `
  -Body '{"email":"jane@example.com","password":"StrongPass123"}'
```

These examples demonstrate the request shapes; successful database access requires a configured SQL Server connection, and the current JWT and protected-endpoint limitations above still apply.

## Current project structure

```text
Authentication&Authorization/
├── MyApp.API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   └── UserController.cs
│   ├── Program.cs
│   └── appsettings.json
├── MyApp.Application/
│   ├── Behavior/                  # MediatR validation pipeline
│   ├── DTOs/
│   ├── Features/
│   │   ├── Authentication/
│   │   ├── Registration/
│   │   └── Users/FetchAll/
│   ├── Interfaces/
│   ├── Permissions/
│   └── Services/
├── MyApp.Domain/
│   └── Entities/                  # Users, roles, permissions, associations
├── MyApp.Infrastructure/
│   ├── Migrations/
│   ├── Persistence/
│   └── Repository/
└── README.md
```

## Known gaps

- **Authentication middleware is missing:** `Program.cs` configures JWT bearer authentication but calls `UseAuthorization()` without first calling `UseAuthentication()`. As a result, bearer tokens are not authenticated before authorization evaluates protected endpoints.
- **Permission claims are not issued:** JWT generation currently adds user ID, name, and email claims only. The permission handler requires a `permission` claim, so the `user:update` policy cannot currently be satisfied by the generated token.
- **JWT serialization should be corrected:** The token generator assigns `JwtSecurityToken.ToString()` to the response token instead of serializing it with `JwtSecurityTokenHandler.WriteToken`. Do not assume the returned value is a usable compact bearer token until this is corrected and verified.
- **RBAC persistence and assignment are incomplete:** The domain has `UserRole` and `RolePermission` entities, but there is no role/permission management or seeding flow. The initial migration creates only `Users`, and the EF model does not yet configure all user-role relationships.
- **Token duration metadata is mislabeled:** `TokenValidityMin` is populated using seconds, not minutes.
- **Login responses do not use HTTP failure status codes:** `AuthController.Login` returns `Ok(...)` for both login success and failure.
- **Authentication error responses need hardening:** The login handler distinguishes invalid email from invalid password and returns exception messages. Use a generic client-facing failure message and server-side logging before exposing this endpoint beyond development.

## License

This project is provided as-is for educational and development purposes.

## Author

Created and maintained by [aldecyrus084](https://github.com/aldecyrus084).
