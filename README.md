🐝 Hive-HQ | Workspace & Inventory Management System
Hive-HQ is an enterprise-grade backend system designed for managing shared workspaces and real-time inventory. Built with .NET 10 and Clean Architecture, it demonstrates high-performance patterns like Distributed Caching (Redis) and Background Task Orchestration (Hangfire).

🚀 Key Technical Features
Clean Architecture (DDD Principles): Separation of concerns ensuring the business logic (Domain) remains independent of frameworks and databases.

Cache-Aside Pattern with Redis: Drastically reduced latency for dashboard statistics by caching high-traffic data.

Automated Background Processing: * Midnight Cleanup: Automatically archives stale "Pending" orders.

Low Stock Alerts: Hourly background checks to notify staff of inventory levels.

Daily Financial Reports: Aggregates revenue data at EOD for permanent storage.

Strongly-Typed Configuration: Implemented the Options Pattern for secure and manageable infrastructure settings.

Robust Testing: Core business rules are verified with xUnit and Moq to ensure zero regressions.

🛠 Tech Stack
Backend: ASP.NET Core 10 (Web API)

Database: PostgreSQL

Caching: Redis

Background Jobs: Hangfire

Validation: FluentValidation (Auto-validation middleware)

Documentation: Swagger/OpenAPI with JWT integration

Containerization: Docker & Docker Compose

📁 Project Structure
Plaintext

src/
├── HiveHQ.Domain/         # Entities, Interfaces, Logic (Zero Dependencies)
├── HiveHQ.Application/    # DTOs, Mapping, Validators, Services
├── HiveHQ.Infrastructure/ # Persistence (EF Core), Redis, Configuration
└── HiveHQ.API/            # Controllers, Middleware, Program.cs
tests/
└── HiveHQ.Tests/          # xUnit & Moq Unit Tests

🚦 Getting Started
Prerequisites
.NET 10 SDK

Docker Desktop

1. Spin up the Infrastructure
   docker-compose up -d

3. Apply Migrations
   dotnet ef database update --project src/HiveHQ.Infrastructure --startup-project src/HiveHQ.API

4. Run the Application
   dotnet run --project src/HiveHQ.API

Note: Access the API documentation at https://localhost:7250/swagger

🧪 Running Tests
   dotnet test
