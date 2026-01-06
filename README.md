# Hive-HQ Enterprise Management System

A robust, scalable backend designed to manage business center operations, inventory, and automated billing. Built with **.NET 9** and **PostgreSQL** using **Clean Architecture** principles.

## 🏗️ Technical Architecture
The project is organized into four distinct layers to ensure maintainability and separation of concerns:
* **Domain:** Core entities and business rules (POCOs).
* **Application:** Interfaces, DTOs, and request validation (FluentValidation).
* **Infrastructure:** Data persistence via Entity Framework Core and PostgreSQL.
* **API:** RESTful endpoints and middleware configuration.

## 🚀 Current Features
- [x] **Clean Architecture** project structure.
- [x] **PostgreSQL Integration** with EF Core Code-First migrations.
- [x] **Generic Repository Pattern** for decoupled data access.
- [x] **Automated Validation** using FluentValidation to ensure data integrity.
- [x] **OpenAPI (Swagger)** documentation for endpoint testing.

## 🛠️ Tech Stack
- **Framework:** .NET 9
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Validation:** FluentValidation
- **IDE:** JetBrains Rider

## 🚦 Getting Started
1. Clone the repo.
2. Update `appsettings.json` with your PostgreSQL credentials.
3. Run `dotnet ef database update` to sync the schema.
4. Run `dotnet run --project src/HiveHQ.API` to start the server.
