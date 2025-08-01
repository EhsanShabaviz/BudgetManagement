# 📘 BudgetManagement

## 🎯 Project Overview

A Blazor Server web application to manage and calculate government project budgets (adjustment amount, required credit, credit deficit, etc.) with a clean, testable architecture.

## 🏗 Architecture

**Clean Architecture** with layered separation:

- **Domain**: Business entities only (no dependencies)
- **Application**: UseCases, Interfaces, DTOs
- **Infrastructure**: EF Core, Repositories, Excel/File Readers
- **Web (Presentation)**: UI Layer with Blazor Server
- **Tests**: Unit and integration tests

## 📂 Project Structure

```
/src
├── BudgetManagement.Domain
│   └── Entities, Interfaces
├── BudgetManagement.Application
│   └── UseCases, Interfaces, DTOs, Services
├── BudgetManagement.Infrastructure
│   └── Repositories, Configuration, Services
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

## 🔐 Auth (upcoming)

- ASP.NET Core Identity
- Role-based authorization

## 🚧 Current Phase

✅ Faza 1 Completed:

- `BudgetRecord` entity
- `IBudgetRepository` contract
- Unit tests

## 📌 Next Steps

- Phase 2: Excel import UseCase
- Infrastructure EF configuration
- DI registration per layer

## 👤 Authors

- Ehsan Shabaviz

