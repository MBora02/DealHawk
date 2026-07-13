# 🦅 DealHawk - Game Price Tracker & Deal Monitoring Platform

[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-10.0-purple.svg)](https://learn.microsoft.com/en-us/ef/core/)
[![Database](https://img.shields.io/badge/Database-SQL%20Server-red.svg)](https://www.microsoft.com/en-us/sql-server/)
[![Background Jobs](https://img.shields.io/badge/Jobs-Hangfire-orange.svg)](https://www.hangfire.io/)
[![API Docs](https://img.shields.io/badge/Docs-Scalar-green.svg)](https://scalar.com/)

DealHawk is a high-performance web application designed for gaming enthusiasts to track game prices, monitor discounts across multiple digital stores, set personalized price alerts, and share reviews. Built on Onion (Clean) Architecture principles using .NET 10, the platform synchronizes real-time deals from external APIs and runs background processing tasks to notify users of price drops.

---

## 📝 Table of Contents

1. [Project Summary](#-project-summary)
2. [Project Overview](#-project-overview)
3. [Architecture](#-architecture)
4. [Features](#-features)
5. [Technology Stack](#-technology-stack)
6. [Project Structure](#-project-structure)
7. [Database Design](#-database-design)
8. [Installation & Setup](#-installation--setup)
9. [Authentication & Authorization](#-authentication--authorization)
10. [API Endpoints](#-api-endpoints)
11. [Future Improvements](#-future-improvements)
12. [License](#-license)
13. [Author](#-author)

---

## 🔍 Project Summary

DealHawk is a game price comparison and deal tracking platform that aggregates pricing data from multiple online gaming storefronts. Designed for budget-conscious gamers, the application enables users to search for games, analyze historical price trends, configure custom price drop alerts, and manage personal wishlists. The platform is powered by ASP.NET Core 10.0, combining a robust CQRS-based REST API with a fully-responsive MVC web interface. By providing centralized real-time discounts and automated background deal checking, DealHawk saves gamers time and money.

---

## 🚀 Project Overview

The gaming market is highly fragmented, with discounts scattered across platforms like Steam, GOG, Epic Games Store, Xbox, and PlayStation. Users often miss the lowest prices because checking each storefront manually is tedious.

DealHawk solves this problem by consolidating pricing metadata into a single, unified database. The system workflows operate as follows:

1. **Aggregated Catalog:** Gamers browse or search the catalog, filtering by genre or platform.
2. **Real-time Price Tracking:** Game pages display the current price, original retail price, and savings percentage across all participating stores.
3. **Price Analytics:** Historical price details are stored to chart price trends over time, helping users decide whether to buy immediately or wait.
4. **Wishlist & Alert Management:** Authenticated users add games to their wishlist or set specific price thresholds. 
5. **Automated Monitoring:** A background scheduler periodically runs jobs that synchronize the latest storefront data via external APIs. If a game's price falls below a user's target threshold, a notification is generated and delivered to their in-app inbox.
6. **Community Feedback:** Gamers submit ratings and written reviews, which undergo moderator approval before appearing publicly.

---

## 🏛️ Architecture

DealHawk is designed using the **Onion Architecture** (Clean Architecture) pattern. This approach isolates the core business rules and domain logic from external concerns, such as database engines, user interface frameworks, and third-party APIs. 

### Why Onion Architecture?
* **Independence of Frameworks:** The core business layers do not depend on external libraries, making updates simple.
* **Database Agnosticism:** Core interfaces abstract data access. EF Core is utilized for persistence, but it could be swapped with minimal impact on domain logic.
* **Testability:** Business rules can be unit tested without requiring real web servers or database connections.
* **Separation of Concerns:** Distinct layers handle data representation, business services, scheduling, and view rendering.


### Layer Responsibilities

1. **DealHawk.Domain (Core Inner Layer):** Contains domain entities, value objects, and enums representing the core domain concepts (e.g., `Game`, `CurrentPrice`, `PriceAlert`, `Review`). It has zero dependencies on other projects or external packages.
2. **DealHawk.Application:** Defines system use cases using the **CQRS (Command Query Responsibility Segregation)** pattern via **MediatR**. Contains command/query handlers, data transfer objects (DTOs), mapping configurations (AutoMapper), validation rules (FluentValidation), and core interfaces (e.g., `IUnitOfWork`, `ICheapSharkService`).
3. **DealHawk.Persistence (Outer Layer):** Coordinates data storage. Configures Entity Framework Core, applies database migrations to SQL Server, and implements repository contracts. It also includes the `AdoNetStatsService` which uses optimized raw SQL/ADO.NET queries to calculate admin dashboard metrics.
4. **DealHawk.Infrastructure (Outer Layer):** Connects to external interfaces. Implements the `CheapSharkService` to communicate with the CheapShark API, generates tokens via the JWT service, implements in-memory caching, and configures **Hangfire** servers for recurring background price sync jobs.
5. **DealHawk.API (Presentation Layer):** A RESTful Web API exposing endpoints for authenticated requests. Includes exception handling middleware, custom CORS configurations, and hosts the interactive **Scalar API Reference** playground.
6. **DealHawk.WebMVC (Presentation Layer):** An ASP.NET Core MVC frontend that consumes the API via a typed `DealHawkApiClient`. It uses cookie-based authentication, Bootstrap 5 for visual elements, and FontAwesome for iconography.

---

## ✨ Features

### 1. Catalog Search & Filtering
Users can search the aggregated gaming database by title, filter by platforms (e.g., Steam, Epic Games, GOG), or narrow down selections by specific genres.
* **Benefit:** Allows users to easily locate games and view deals without navigating multiple platforms.

<img width="1863" height="838" alt="image" src="https://github.com/user-attachments/assets/00541609-7af0-4dc2-985c-45b66790db47" />
<img width="1390" height="496" alt="image" src="https://github.com/user-attachments/assets/ef823208-c2d8-4c7a-bcca-c14e408b4269" />
<img width="1383" height="477" alt="image" src="https://github.com/user-attachments/assets/5403a5ac-27c1-4833-a39b-9c8f35d7e6ee" />


---

### 2. Game Details, Price Analytics, History & Reviews
<img width="852" height="853" alt="image" src="https://github.com/user-attachments/assets/b0aafcf6-29b0-4f0b-b486-48d2841dc82b" />
<img width="1110" height="672" alt="image" src="https://github.com/user-attachments/assets/25525a5e-1e24-46c6-946b-267d17803f1a" />
<img width="937" height="485" alt="image" src="https://github.com/user-attachments/assets/1c3cea44-9a09-4dff-9dd7-e8f1edbfee75" />


Clicking any game displays a comprehensive details screen including current storefront prices, developer metadata, historical lowest price records, and a price timeline.
* **Benefit:** Empowers gamers to check if a current deal is genuinely the lowest price ever offered or if they should wait for a better discount.


---

### 3. User Dashboard & Price Alerts
<img width="902" height="618" alt="image" src="https://github.com/user-attachments/assets/eaca53ef-b0ec-4ce3-aaf8-be7017169f3e" />
<img width="738" height="520" alt="image" src="https://github.com/user-attachments/assets/91ca14af-96d6-490b-9623-b8df26715c46" />
<img width="1342" height="346" alt="image" src="https://github.com/user-attachments/assets/b97de3b1-31cd-400b-975c-1349a897aa64" />
<img width="1346" height="392" alt="image" src="https://github.com/user-attachments/assets/adb105e0-0d2f-49a4-a6e6-3567e1a00064" />

---

### 4. Moderator Navbar & Control Panel
<img width="1372" height="80" alt="image" src="https://github.com/user-attachments/assets/e4a54982-ad8f-4e48-b64d-56655c21dfc0" />
<img width="1356" height="372" alt="image" src="https://github.com/user-attachments/assets/c5460bfb-175d-4314-a08e-e61167840e12" />
<img width="905" height="302" alt="image" src="https://github.com/user-attachments/assets/4d30b995-2a99-4edd-890f-e034d028d513" />

---

### 5. Admin Management Navbar and Control Panel
<img width="1335" height="53" alt="image" src="https://github.com/user-attachments/assets/cae984b6-83d1-45aa-b1c4-b245f8862ec6" />
<img width="1106" height="812" alt="image" src="https://github.com/user-attachments/assets/6feac0ea-1424-4703-a80f-6a584cc0b550" />
<img width="1265" height="540" alt="image" src="https://github.com/user-attachments/assets/c640621a-b9ac-4709-86d5-f3572a57baf3" />
<img width="1247" height="830" alt="image" src="https://github.com/user-attachments/assets/66eb234d-34aa-432d-ac74-60d2387b6435" />
<img width="1352" height="742" alt="image" src="https://github.com/user-attachments/assets/94d21179-5215-4ef9-8fb5-c653f6505a8d" />
<img width="1335" height="845" alt="image" src="https://github.com/user-attachments/assets/a0bf177e-5c10-4e53-9d1d-a684188e72e3" />
<img width="1323" height="571" alt="image" src="https://github.com/user-attachments/assets/e7ccf396-1af1-4ec9-97b6-4450e96c4384" />
<img width="1348" height="585" alt="image" src="https://github.com/user-attachments/assets/ce9e9047-8b82-4bc8-8937-89c4332644ba" />

---

### 6. Price Sync from CheapSharkAPI and Importing bulk games and admin single game import
<img width="897" height="536" alt="image" src="https://github.com/user-attachments/assets/c0ca7c60-5a75-4a00-a08b-5a3c72569105" />
<img width="461" height="332" alt="image" src="https://github.com/user-attachments/assets/079f9c53-19c5-43ac-809d-4b51ce2466e5" />

---

### 7. Hangfire Dashboard for Background Jobs
<img width="1880" height="630" alt="image" src="https://github.com/user-attachments/assets/4db37222-123c-46d1-a16f-a22ebfc3f9bb" />
<img width="1887" height="413" alt="image" src="https://github.com/user-attachments/assets/6c437a3c-baee-4d79-9a31-b9e8cfd10cc6" />
<img width="1896" height="336" alt="image" src="https://github.com/user-attachments/assets/0c85d5fe-8095-475d-a662-ef0d6797a664" />




---

## 🛠️ Technology Stack

| Category | Technology Used |
| :--- | :--- |
| **Backend Framework** | ASP.NET Core 10.0 (Web API & MVC) |
| **Frontend UI** | Razor Views, Bootstrap 5, FontAwesome 6, Custom Vanilla CSS |
| **Database Provider** | Microsoft SQL Server (LocalDB in Development) |
| **ORM** | Entity Framework Core 10.0 |
| **Authentication** | ASP.NET Core Identity (IdentityDbContext) |
| **Authorization** | Role-based Authorization (JWT Bearer for API, Cookies for MVC) |
| **Mapping Library** | AutoMapper (v13.0.1) |
| **Validation Library** | FluentValidation (v12.1.1) |
| **Background Processing**| Hangfire (v1.8.23) with SQL Server Storage |
| **Logging** | Serilog (v10.0.0) with Console and File Sinks |
| **Caching** | Microsoft.Extensions.Caching.Memory (In-Memory Cache) |
| **API Documentation** | Microsoft.AspNetCore.OpenApi, Scalar API Reference (DeepSpace theme) |
| **External Integration**| CheapShark REST API Integration (via HTTPClientFactory) |

---

## 📁 Project Structure

Below is the directory tree of the DealHawk solution:

```
DealHawk/
│
├── DealHawk.slnx                     # Visual Studio XML-based Solution file
│
├── DealHawk.Domain/                  # Core domain models (no dependencies)
│   ├── Entities/                     # Database models (Game, User, PriceAlert, etc.)
│   └── Enums/                        # System enums (ReviewStatus, SyncStatus)
│
├── DealHawk.Application/             # Application Business Logic
│   ├── DTOs/                         # Data Transfer Objects
│   ├── Features/                     # CQRS Commands & Queries (Auth, Games, etc.)
│   ├── Interfaces/                   # Abstractions (IUnitOfWork, ICacheService, etc.)
│   ├── Mapping/                      # AutoMapper profiles
│   └── Validators/                   # FluentValidation classes
│
├── DealHawk.Infrastructure/          # Core external service integrations
│   ├── Jobs/                         # Hangfire background jobs (PriceSyncJob)
│   └── Services/                     # CacheService, CheapSharkService, JwtTokenGenerator
│
├── DealHawk.Persistence/             # Database Access & Configuration
│   ├── Context/                      # ApplicationDbContext & DatabaseSeeder
│   ├── Migrations/                   # EF Core database migrations
│   ├── Repositories/                 # GenericRepository & UnitOfWork implementations
│   └── Services/                     # AdoNetStatsService using optimized ADO.NET
│
├── DealHawk.API/                     # REST API Backend
│   ├── Controllers/                  # API Controllers (Auth, Games, Admin, Alerts, etc.)
│   ├── Middlewares/                  # ExceptionHandlingMiddleware
│   └── Program.cs                    # API Setup, Dependency Injection, Swagger
│
└── DealHawk.WebMVC/                  # MVC Web UI Frontend
    ├── Controllers/                  # MVC Controllers (Dashboard, Moderation, Admin)
    ├── Services/                     # DealHawkApiClient for API communication
    ├── Views/                        # Razor Pages layout and views
    └── wwwroot/                      # Static assets (CSS, JS, images)
```

---

## 🗄️ Database Design

DealHawk uses a relational database structured around game data, pricing history, and user interactions.

```
                  ┌──────────────┐         ┌──────────────┐
                  │  Publisher   │         │    Genre     │
                  └──────┬───────┘         └──────┬───────┘
                         │ 1                      │ 1
                         │                        │
                         │ 1..*                   │ 1..*
                  ┌──────┴───────┐         ┌──────┴───────┐
                  │     Game     ├─────────┤  GameGenre   │
                  └──────┬───────┘         └──────────────┘
                         │ 1
                         ├────────────────────────┬────────────────────────┐
                         │ 1..*                   │ 1..*                   │ 1..*
                  ┌──────┴───────┐         ┌──────┴───────┐         ┌──────┴───────┐
                  │  StoreGame   │         │    Review    │         │  PriceAlert  │
                  └──────┬───────┘         └──────┬───────┘         └──────┬───────┘
                         │ 1                      │ 1                      │ 1
                         │                        │                        │
                         │ 1..*                   │ 1..*                   │ 1..*
                  ┌──────┴───────┐         ┌──────┴───────┐         ┌──────┴───────┐
                  │ CurrentPrice │         │  ReviewLike  │         │ Notification │
                  └──────────────┘         └──────────────┘         └──────────────┘
```

### Main Entities & Relationships:
* **Game:** The central entity storing basic details (Title, CheapShark ID, Release Date).
* **Publisher:** Links one publisher to many games (One-to-Many).
* **Genre & Platform:** Associated with games using junction tables (`GameGenre` and `GamePlatform`) to support Many-to-Many relationships.
* **Store & StoreGame:** A store represents a vendor (e.g., Steam, Epic). `StoreGame` maps a game to its respective store page and identifiers.
* **CurrentPrice:** Associated with `StoreGame`. Stores the dynamic current sale price, standard retail price, and calculations of savings.
* **PriceHistory:** Stores historical snapshots of game prices across stores to render tracking charts.
* **ApplicationUser:** Represents register users. Has collections of wishlists, favorites, reviews, notifications, and audit activities.
* **Wishlist & Favorite:** Relates users to games. Wishlist includes an optional target price.
* **PriceAlert:** Connects users to games with a specific target price. Set to trigger when a store's price goes below the target threshold.
* **Review & ReviewLike:** User-submitted ratings and text comments, moderated before display. Supports review likes by other users.
* **AuditLog:** Stores security and management logs for admin actions.
* **StoreSyncLog:** Records details about background data sync jobs (runtime, execution status, count of updated games, and errors).

---

## 🛠️ Installation & Setup

Follow these steps to run the DealHawk API and WebMVC projects locally:

### 📋 Prerequisites
* [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
* [SQL Server Express / LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
* Visual Studio 2022 or VS Code

### 1. Clone the Repository
```bash
git clone https://github.com/your-username/DealHawk.git
cd DealHawk
```

### 2. Restore Dependencies
Restore Nuget packages across the solution:
```bash
dotnet restore
```

### 3. Configure Database
By default, the persistence assembly targets SQL Server LocalDB.
Open `DealHawk.API/appsettings.json` and update the connection string if you are running a custom SQL Server Instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DealHawkDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 4. Apply Database Migrations
Create the database tables and schema configurations:
```bash
dotnet ef database update --project DealHawk.Persistence --startup-project DealHawk.API
```

### 5. Run the Applications
Open two terminal windows to run both the API backend and the MVC frontend simultaneously.

**Start the Web API:**
```bash
cd DealHawk.API
dotnet run
```
* The API will launch on: `https://localhost:7198` and `http://localhost:5233` (verify ports in launchSettings).
* Access the **Scalar API documentation** at `http://localhost:5233/scalar/v1`.

**Start the Web MVC Frontend:**
```bash
cd ../DealHawk.WebMVC
dotnet run
```
* The web portal will run on `https://localhost:7290` and `http://localhost:5030`.
* Open your browser and navigate to `http://localhost:5030` to access the site.

---

## 🔐 Authentication & Authorization

DealHawk utilizes a decoupled authentication flow between the MVC UI and REST API.

```
┌──────────────┐                 ┌──────────────┐                 ┌──────────────┐
│  DealHawk    │                 │   DealHawk   │                 │   DealHawk   │
│    WebMVC    │                 │  API Backend │                 │   Identity   │
└──────┬───────┘                 └──────┬───────┘                 └──────┬───────┘
       │                                │                                │
       │ Submit Login (credentials)     │                                │
       ├───────────────────────────────>│                                │
       │                                │ Validate & Fetch User          │
       │                                ├───────────────────────────────>│
       │                                │ <──────────────────────────────┤
       │                                │ User details & Roles           │
       │                                │                                │
       │                                │ Generate JWT                   │
       │                                ├──┐                             │
       │                                │  │                             │
       │                                <──┘                             │
       │ Return JWT, Username, Roles    │                                │
       │<───────────────────────────────┤                                │
       │                                │                                │
       │ Store Token in Cookie Claims   │                                │
       ├──┐                             │                                │
       │  │                             │                                │
       <──┘                             │                                │
       │                                │                                │
       │ Request Data (JWT Bearer)      │                                │
       ├───────────────────────────────>│                                │
       │                                │ Authorize & Return Data        │
       │<───────────────────────────────┤                                │
```

### Flow Details:
1. **API JWT Auth:** The API backend executes authentication using **ASP.NET Core Identity** and generates a signed JWT token on login (`api/auth/login`).
2. **MVC Cookie Auth:** The MVC project handles session state locally using **Cookie Authentication**. Upon logging in, the MVC controller forwards user credentials to the API, retrieves the resulting JWT token, and stores it as a claim within the MVC cookie.
3. **API Client Header Forwarding:** For all subsequent requests, `DealHawkApiClient` retrieves the JWT token from the logged-in user claims and appends it to the outgoing HTTP request headers:
   `Authorization: Bearer {Token}`.

### Access Levels (Roles):
* **Admin:** Unrestricted access. Can view full system statistics, inspect security audit logs, trigger background jobs, import games, and manage user listings.
* **Moderator:** Access to the moderation dashboard to approve or reject user-submitted game reviews.
* **User (Authenticated):** Can add items to favorites or wishlists, configure price alerts, submit reviews for approval, and mark price drop notifications as read.
* **Guest (Unauthenticated):** Restricted to searching the game catalog and viewing basic detail pages.

---

## 🔌 API Endpoints

The API is fully documented through the **Scalar API Playground**. Below is a summary of the main endpoints:
Note:There are more but these are the main endpoints

### Auth Endpoints (`api/auth`)
* `POST /register` - Register a new user account.
* `POST /login` - Log in and obtain a JWT Token.
* `GET /me` - Retrieve current user profile and role details (requires authorization).

### Game Endpoints (`api/games`)
* `GET /` - Retrieve a filtered, paginated list of games.
* `GET /{id}` - Get comprehensive details of a single game.
* `GET /{id}/history` - Get historical pricing snapshots for a game.
* `POST /import` - Import a new game from the CheapShark index (Admin only).

### Personalization Endpoints
* `GET /wishlist` - View current user's wishlist items.
* `POST /wishlist/toggle` - Add or remove games from the user's wishlist (supports setting target prices).
* `GET /favorites` - View user's favorites list.
* `POST /favorites/toggle` - Add or remove games from favorites.
* `GET /alerts` - View user's configured price drop alerts.
* `POST /alerts` - Configure a new target price alert for a game.
* `DELETE /alerts/{id}` - Remove an active price alert.
* `GET /notifications` - Retrieve list of price drop alerts and system notifications.
* `POST /notifications/mark-read` - Clear notifications.

### Review Endpoints (`api/reviews`)
* `POST /` - Submit a game review (Requires Moderator approval before displaying).
* `POST /{id}/like` - Upvote/Like an existing review.
* `GET /pending` - Retrieve reviews awaiting moderation (Moderator/Admin only).
* `POST /{id}/approve` - Approve a review (Moderator/Admin only).
* `POST /{id}/reject` - Reject a review (Moderator/Admin only).

### Administrative Portal (`api/admin`)
* `GET /dashboard-stats` - Retrieve key metric counts and system status (compiled via raw SQL).
* `POST /sync` - Force a background storefront/price synchronization run.
* `GET /audit-logs` - Inspect logs detailing admin/moderation histories.
* `GET /sync-logs` - Inspect background job history.
* `POST /import-bulk` - Enqueue bulk imports of game indices.

---

## 🔮 Future Improvements

Proposed enhancements to build upon the current codebase:
* **Redis Caching:** Introduce distributed caching for game listing and detail pages to reduce database load and improve response times.
* **SignalR Real-Time Alerts:** Replace pull notifications with real-time push alerts to notify active web dashboard users instantly when a price drop occurs.
* **Elasticsearch Catalog Search:** Replace SQL string matching with an Elasticsearch index for faster, fuzzy search results on games.
* **OAuth Login Integrations:** Support logging in with third-party providers (Steam, Discord, Google).
* **Comprehensive Testing:** Add xUnit unit tests for MediatR queries/commands and integration tests for MVC controller routing.
* **CI/CD Pipelines:** Set up GitHub Actions workflow to build, lint, test, and package applications into Docker containers.

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](file:///c:/Users/Bora/Desktop/DealHawk/LICENSE) file for details.


