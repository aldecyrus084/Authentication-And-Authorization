# Authentication & Authorization System

A comprehensive .NET 10 Web API project demonstrating secure user authentication and authorization using JWT (JSON Web Tokens). This project follows clean architecture principles with a layered approach featuring the Application, Domain, API, and Infrastructure layers.

## 📋 Project Overview

This project implements a production-ready authentication system with the following architecture:

- **MyApp.API** - REST API layer with ASP.NET Core controllers
- **MyApp.Application** - Business logic, services, and MediatR handlers
- **MyApp.Infrastructure** - Data access, repositories, and persistence
- **MyApp.Domain** - Core entities and domain models

## 🔐 Authentication Implementation

### Key Authentication Features

The authentication system is built on the following components:

#### 1. **JWT Token Generation**
- **TokenGenerator** - Generates secure JWT tokens for authenticated users
- **JwtSettings** - Configuration class for JWT parameters
  - **Secret**: Secret key for signing tokens
  - **Issuer**: Token issuer identifier
  - **Audience**: Token audience identifier
  - **ExpirationMinute**: Token validity period in minutes

#### 2. **Password Security**
- Uses **BCrypt.Net** for secure password hashing and verification
- Passwords are never stored in plain text
- Password verification is performed using BCrypt comparison

#### 3. **User Authentication Flow**
The authentication process follows these steps:
1. User sends email and password to the login endpoint
2. `AuthHandler` (MediatR handler) processes the authentication request
3. User is retrieved from database by email
4. Password is verified using BCrypt
5. JWT token is generated with user claims
6. Token and user information are returned to the client

#### 4. **Token Claims**
JWT tokens include the following user claims:
- **NameIdentifier**: User ID (UserId)
- **Name**: User's full name
- **Email**: User's email address

### Authentication API Endpoint

**POST** `api/auth`

Request Body:
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

Response (Success):
```json
{
  "isSuccess": true,
  "message": "Login Successfully",
  "data": {
	"userId": "550e8400-e29b-41d4-a716-446655440000",
	"name": "John Doe",
	"email": "user@example.com",
	"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

Response (Failure):
```json
{
  "isSuccess": false,
  "message": "Email or Password invalid."
}
```

## 🏗️ Architecture Components

### Core Authentication Classes

#### **AuthCommand** (Location: `MyApp.Application/Features/Authentication/`)
- MediatR IRequest implementation
- Contains Email and Password properties
- Triggers the authentication handler

#### **AuthHandler** (Location: `MyApp.Application/Features/Authentication/`)
- Implements `IRequestHandler<AuthCommand, GenericResponseDTO<AuthResponseDTO>>`
- Main business logic for user authentication
- Password verification and token generation coordination

#### **TokenGenerator** (Location: `MyApp.Application/Services/`)
- Implements `ITokenGenerator` interface
- Creates signed JWT tokens
- Uses HS256 (HMAC SHA-256) signing algorithm

#### **JwtSettings** (Location: `MyApp.Application/Services/`)
- Configuration class for JWT parameters
- Injected as singleton in dependency injection

#### **ITokenGenerator** (Location: `MyApp.Application/Interfaces/Services/`)
- Service interface for token generation
- Defines `GenerateToken(Users user)` contract

#### **IUserRepository** (Location: `MyApp.Application/Interfaces/Repository/`)
- Repository interface for user data access
- Provides `GetUserByEmailAsync(string email)` method

## 🛠️ Setup & Installation

### Prerequisites
- .NET 10 SDK or later
- Visual Studio 2026 (or compatible IDE)
- SQL Server or other supported database

### Dependencies

Key NuGet packages used:
- **MediatR** - CQRS and command handling
- **BCrypt.Net-Next** - Password hashing and verification
- **System.IdentityModel.Tokens.Jwt** - JWT token creation and validation
- **Microsoft.IdentityModel.Tokens** - Token security and signing
- **Entity Framework Core** - Database access (in Infrastructure layer)

### Configuration

#### 1. JWT Settings Configuration
Update `appsettings.json` with your JWT configuration:

```json
{
  "JwtSettings": {
	"Secret": "your-very-secure-secret-key-min-32-chars",
	"Issuer": "your-app-name",
	"Audience": "your-app-users",
	"ExpirationMinute": 60
  }
}
```

**Important**: 
- Secret key should be at least 32 characters long
- Use strong, random values for production
- Store sensitive values in environment variables or Azure Key Vault

#### 2. Database Connection
Configure your database connection in `appsettings.json`:

```json
{
  "ConnectionString": {
	"DefaultConnection": "Server=your-server;Database=YourDB;User Id=sa;Password=yourpassword;"
  }
}
```

#### 3. Dependency Injection Setup
The `Program.cs` file automatically registers:
- JWT Settings service
- MediatR handlers
- Repository implementations
- Token generator service

### Running the Application

1. Clone the repository:
```bash
git clone https://github.com/aldecyrus084/Authentication-And-Authorization.git
cd Authentication-And-Authorization
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Update database (if using EF Core migrations):
```bash
dotnet ef database update
```

4. Run the application:
```bash
dotnet run --project MyApp.API
```

5. Access the API:
- Swagger UI: `https://localhost:5001/openapi`
- Authentication endpoint: `POST https://localhost:5001/api/auth`

## 🔑 Important Implementation Details

### 1. **Password Hashing**
Passwords are hashed using BCrypt before storing in the database:
```csharp
var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
```

### 2. **Password Verification**
During login, the provided password is verified against the stored hash:
```csharp
var isValid = BCrypt.Net.BCrypt.Verify(providedPassword, storedHash);
```

### 3. **Token Creation Process**
The token generation follows these steps:
1. Create symmetric security key from JWT secret
2. Create signing credentials using HMAC-SHA256
3. Prepare user claims (ID, Name, Email)
4. Create JWT token with issuer, audience, and expiration
5. Convert token to string format

### 4. **Error Handling**
- Invalid email returns: "Email or Password invalid."
- Invalid password returns: "Email or Password invalid."
- Exceptions are caught and returned as error responses
- Prevents information leakage about account existence

### 5. **Token Expiration**
- Tokens expire based on `ExpirationMinute` setting
- Default: 60 minutes
- Expired tokens will be rejected by validation middleware

## 📁 Project Structure

```
Authentication&Authorization/
├── MyApp.API/
│   ├── Controllers/
│   │   └── AuthController.cs          # Authentication endpoint
│   ├── Program.cs                     # Service configuration
│   ├── appsettings.json              # Configuration settings
│   └── Properties/
│       └── launchSettings.json       # Launch profiles
│
├── MyApp.Application/
│   ├── Features/
│   │   └── Authentication/
│   │       ├── AuthCommand.cs        # Login request
│   │       └── AuthHandler.cs        # Authentication logic
│   ├── Interfaces/
│   │   ├── Repository/
│   │   │   └── IUserRepository.cs    # User data access contract
│   │   └── Services/
│   │       └── ITokenGenerator.cs    # Token generation contract
│   ├── Services/
│   │   ├── TokenGenerator.cs         # JWT token implementation
│   │   └── JwtSettings.cs            # JWT configuration
│   └── DTOs/
│       ├── AuthResponseDTO.cs        # Login response
│       └── GenericResponseDTO.cs     # Generic response wrapper
│
├── MyApp.Infrastructure/
│   ├── Persistence/
│   │   └── ApplicationDbContext.cs   # Database context
│   ├── Repository/
│   │   └── UserRepository.cs         # User data implementation
│   └── DependencyInjection.cs        # Infrastructure services
│
├── MyApp.Domain/
│   └── Entities/
│       └── Users.cs                  # User entity
│
└── README.md
```

## 🔄 Authentication Flow Diagram

```
Client Request (Email + Password)
	↓
AuthController.Login()
	↓
MediatR.Send(AuthCommand)
	↓
AuthHandler.Handle()
	├─→ UserRepository.GetUserByEmailAsync()
	├─→ BCrypt.Verify(Password)
	├─→ TokenGenerator.GenerateToken()
	└─→ Return AuthResponseDTO with Token
	↓
Client Receives Token
```

## 🧪 Testing the Authentication

### Using Postman or cURL

```bash
curl -X POST https://localhost:5001/api/auth \
  -H "Content-Type: application/json" \
  -d '{
	"email": "user@example.com",
	"password": "securePassword123"
  }'
```

### Using the Generated Token

Include the token in subsequent requests:
```bash
curl -H "Authorization: Bearer {token}" \
  https://localhost:5001/api/protected-endpoint
```

## 🚀 Best Practices Implemented

1. ✅ **Secure Password Storage** - BCrypt hashing instead of plain text
2. ✅ **JWT Tokens** - Stateless authentication with signed tokens
3. ✅ **CQRS Pattern** - MediatR for command handling
4. ✅ **Dependency Injection** - Loose coupling and testability
5. ✅ **Repository Pattern** - Abstraction of data access
6. ✅ **Error Handling** - Generic error responses without information leakage
7. ✅ **Clean Architecture** - Separation of concerns across layers
8. ✅ **Configurable Settings** - JWT settings via appsettings.json

## 🔒 Security Recommendations

1. **Never commit secrets** - Use environment variables or secure vaults
2. **Use HTTPS only** - Ensure all authentication traffic is encrypted
3. **Set strong JWT secret** - Minimum 32 characters, use random values
4. **Implement token refresh** - Add refresh token mechanism for better security
5. **Add rate limiting** - Prevent brute force attacks on login endpoint
6. **Validate token** - Always validate JWT tokens on protected endpoints
7. **Use CORS carefully** - Configure CORS policy appropriately
8. **Log authentication events** - Track successful and failed login attempts

## 📝 License

This project is provided as-is for educational and development purposes.

## 👤 Author

Created and maintained by [aldecyrus084](https://github.com/aldecyrus084)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

---

**Last Updated**: 2024
**Target Framework**: .NET 10
**Status**: Active Development
