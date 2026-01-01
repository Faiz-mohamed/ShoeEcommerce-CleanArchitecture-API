# 👞 Shoe Ecommerce API (Clean Architecture)

A production-grade RESTful API for a shoe e-commerce platform, built with .NET 8 and following the principles of Clean Architecture. This project demonstrates advanced backend patterns including CQRS, Domain-Driven Design (DDD), and secure payment integration.

## 🚀 Key Features

### 🏗 Architecture & Design
* **Clean Architecture:** Strict separation of concerns (Domain, Application, Infrastructure, API).
* **CQRS Pattern:** Command Query Responsibility Segregation using custom Handlers.
* **Domain-Driven Design:** Rich domain entities with encapsulated logic.
* **Repository Pattern:** Abstraction over Entity Framework Core.
* **Dependency Injection:** Heavy use of .NET Core's DI container.

### 🛒 E-Commerce Modules
* **Product Catalog:** Supports complex variants (Size, Color, Price), Categories, and Brands.
* **Smart Cart:** Persistent shopping cart with stock validation.
* **Wishlist:** "Toggle" logic for adding/removing favorites.
* **Order Management:** Atomic transactions for order creation and inventory deduction.
* **Payment Gateway:** Full integration with **Razorpay** (Order Creation, Signature Verification, Webhooks).

### 🛡 Security & Identity
* **JWT Authentication:** Secure access/refresh token flow.
* **RBAC:** Role-Based Access Control (Admin vs. Customer).
* **Admin Panel:** User management, blocking mechanisms, and catalog control.
* **Validation:** Robust input validation using **FluentValidation**.

## 🛠 Tech Stack

* **Framework:** .NET 8 / ASP.NET Core Web API
* **Database:** Microsoft SQL Server
* **ORM:** Entity Framework Core (Code-First)
* **Documentation:** Swagger / OpenAPI
* **Validation:** FluentValidation
* **Payments:** Razorpay SDK

## ⚙️ Setup & Installation

1.  **Clone the repository**
    ```bash
    git clone [https://github.com/YOUR_USERNAME/ShoeEcommerce-CleanArchitecture-API.git](https://github.com/YOUR_USERNAME/ShoeEcommerce-CleanArchitecture-API.git)
    ```
2.  **Configure AppSettings**
    * Rename `appsettings.json` and update the placeholders with your SQL Server credentials and Razorpay Keys.
3.  **Database Migration**
    ```bash
    Update-Database
    ```
    *The app includes a `DbInitializer` that will automatically seed dummy products, brands, and categories on first run.*
4.  **Run the API**
    ```bash
    dotnet run
    ```
    Access Swagger documentation at `https://localhost:YOUR_PORT/swagger`.

## 📂 Project Structure

```text
src/
├── ShoeEcommerce.API             # Entry point, Controllers, Middlewares
├── ShoeEcommerce.Application     # Business Logic, CQRS Handlers, Validators, DTOs
├── ShoeEcommerce.Domain          # Entities, Enums, Common Interfaces
└── ShoeEcommerce.Infrastructure  # EF Core, Migrations, External Services (Razorpay)
```

🤝 Contributing
This is a learning project intended to showcase backend architecture skills. Suggestions are welcome!
