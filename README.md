# AuthManager

AuthManager is a .NET Core project that provides authentication and authorization services, including user management, registration, login, and token management.

## Dependencies

This project uses the following third-party libraries:

- [Moq](https://github.com/moq/moq) - A mocking library for .NET (License: BSD-3-Clause)

## Table of Contents

- [Features](#features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
  - [Running the API](#running-the-api)
  - [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)

## Features

- User Registration
- User Login
- JWT Token Generation
- JWT Token Refresh
- User Role Management

## Getting Started

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) (version 5.0 or later)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or another database

### Installation

- Clone the repository:
  ```bash
  git clone https://github.com/your-username/AuthManager.git
  cd AuthManager
  ```

- Set up the database:
  - Update the connection string in `appsettings.json` located in `AuthManager.API`:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=AuthManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

- Set up server adresses:
  - Update the Kestrel in `appsettings.json` located in `AuthManager.API`:
    ```json
      "Kestrel": {
    "Endpoints": {
      "MyHttpEndpoint": {
        "Url": "http://localhost:5000"
      },
      "MyHttpsEndpoint": {
        "Url": "https://localhost:5001"
      }
    }
  }
    ```

- Apply database migrations:
  ```bash
  cd AuthManager.API
  dotnet ef database update
  ```

## Usage

### Running the API

- Navigate to the `AuthManager.API` project:
  ```bash
  cd AuthManager.API
  ```

- Run the API:
  ```bash
  dotnet run
  ```

- The API will be available at `https://localhost:5001` or `http://localhost:5000`.

### API Endpoints

- **POST /api/auth/register**: Register a new user
- **POST /api/auth/login**: Login and obtain a JWT token
- **POST /api/auth/refresh**: Refresh an expired JWT token
- **GET /api/users**: Get a list of users (requires authentication)

## Project Structure

```
AuthManager/
├── AuthManager.API/             # API project
│   ├── Controllers/             # API controllers
│   ├── Services/                # Business logic
│   ├── DAL/                     # Data Access Layer
│   │   ├── Entities/            # Database entities
│   │   ├── Migrations/          # Database migrations
│   │   ├── Repositories/        # Repository interfaces and implementations
│   ├── Models/                  # DTOs used in API
│   └── Program.cs               # Main entry point
└── AuthManager.sln              # Solution file
```

## Testing

Unit tests are located in the `AuthManager.Tests` project. To run the tests, use the following command:

```bash
dotnet test
```

### Example Test

Here's an example of a unit test for the `TokenGenerator` class:

```csharp
[Fact]
public void GenerateToken_ShouldReturnValidJwtToken()
{
    // Arrange
    var user = new IdentityUser { UserName = "testuser" };
    var tokenGenerator = new TokenGenerator(/* configuration setup */);

    // Act
    var token = tokenGenerator.GenerateToken(user);

    // Assert
    Assert.NotNull(token);
    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    Assert.Equal("testuser", jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
    Assert.Equal("your_issuer", jwtToken.Issuer);
    Assert.Equal("your_audience", jwtToken.Audiences.First());
}
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

- Fork the repository
- Create a new branch (`git checkout -b feature-branch`)
- Make your changes
- Commit your changes (`git commit -am 'Add new feature'`)
- Push to the branch (`git push origin feature-branch`)
- Open a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
