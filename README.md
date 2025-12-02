# 📘 BudgetManagement

## 🎯 Project Overview

A Blazor Server web application to manage and calculate government project budgets (adjustment amount, required credit, credit deficit, etc.) with a clean, testable architecture. The project has been fully completed across all planned phases, delivering a secure, extensible, and production-ready solution.

## 🏗 Architecture

**Clean Architecture** with layered separation:

- **Domain**: Business entities only (no dependencies)
- **Application**: UseCases, Interfaces, DTOs
- **Infrastructure**: EF Core, Repositories, Services, Excel/File Readers
- **Common**: Shared extensions and utilities (e.g., Persian date and string helpers)
- **Web (Presentation)**: UI Layer with Blazor Server
- **Tests**: Unit and integration tests

## 📂 Project Structure

```
/src
├── BudgetManagement.Domain
│   └── Entities, Interfaces
├── BudgetManagement.Application
│   └── UseCases, Interfaces, DTOs
├── BudgetManagement.Infrastructure
│   └── Repositories, Configuration, Services
├── BudgetManagement.Common
│   └── PersianDateExtensions.cs, StringExtensions.cs
├── BudgetManagement.Web
│   └── Components, Pages, Services
/tests
└── BudgetManagement.UnitTests
```

## ⚙ Technologies

- C# / .NET 9
- ASP.NET Core / Blazor Server
- SQL Server
- AutoMapper (upcoming)
- xUnit for testing
- Clean Architecture principles

## 🔐 Auth

- ASP.NET Core Identity
- Role-based authorization

## ✅ Project Phases

- **Phase 1**: BudgetRecord entity, IBudgetRepository contract, Unit tests
- **Phase 2**: Excel import UseCase, Infrastructure EF configuration, DI registration per layer
- **Phase 3**: Advanced filtering and reporting (decimal range sliders, print-friendly reports), Localization and RTL support
- **Phase 4**: Deployment automation with LibMan/CDN vs local asset management, Offline enterprise environment support
- **Phase 5** (Final): Security enhancements (IP restrictions, SSL, certificate management, role-based access control), Documentation aligned with regulatory standards

## 🚀 Getting Started

1. Clone the repo: `git clone https://github.com/EhsanShabaviz/BudgetManagement.git`
2. Navigate to `/src/BudgetManagement.Web` and run `dotnet restore`.
3. Update `appsettings.json` with your SQL Server connection string.
4. Run migrations: `dotnet ef database update` (from Infrastructure project).
5. Start the app: `dotnet run`
6. Access at `https://localhost:5001` (or configured port).
For production deployment, configure IIS or use Docker (setup available in docs).

## 👤 Authors

- Ehsan Shabaviz

