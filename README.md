# Modular Monolith & Microservices Marketplace

This project demonstrates the evolution from a **Modular Monolith** to a **Service-Oriented** architecture.
It serves as a practical guide for vertically slicing an application and extracting high-traffic modules into standalone microservices.

---

## 🏗️ Architecture: The Hybrid Approach

We started with a Modular Monolith (one deployment unit), but have now extracted the **Catalog Module** into a separate service to scale independently.

### 1. Distinct Services
-   **App.Web (The Monolith Host)**: Hosts the **User** and **Ordering** modules. It acts as the primary API Gateway for clients.
-   **App.Service.Catalog (The Microservice)**: A dedicated, standalone service hosting the **Catalog** module. It uses **gRPC** for high-performance communication.

### 2. Vertical Slicing 🍰
Regardless of deployment (Monolith vs Microservice), we stick to **Vertical Slicing**:
-   **One Feature = One Folder**.
-   Example: `CreateProduct` folder contains the API Endpoint, Request DTO, Handler logic, and Validator.
-   **Benefit**: You don't hunt for files across layers (Controller/Service/Repo). Everything for a feature is together.

### 3. Communication Patterns 📡

Modules need to talk. Here is how they do it in this hybrid setup:

#### A. Synchronous (gRPC) - "The Direct Line"
When `App.Web` (e.g., the Cart or Admin UI) needs product data, it calls the `App.Service.Catalog` directly.
-   **Protocol**: **gRPC** (HTTP/2, Protobuf).
-   **Why?**: Extremely fast, strongly typed contracts (`.proto` files), and smaller payload size than REST.
-   **Flow**: `App.Web` (Client) -> `CatalogService` (gRPC Server).

#### B. Asynchronous (RabbitMQ) - "The Message Bus"
For side effects and decoupling, we use **MassTransit** over **RabbitMQ**.
-   **Pattern**: Publish/Subscribe.
-   **Example**:
    1.  User places an order in `App.Web`.
    2.  `Ordering` module publishes `OrderCreated` event to RabbitMQ.
    3.  `Catalog` service (listening on `catalog-order-created` queue) receives the event and updates stock.
    4.  `Notification` module (listening on `notification-order-created` queue) receives the event and sends an email.
-   **Benefit**: If `Catalog` service is down, the message waits in the queue. Zero data loss.

---

## 📂 Project Structure

```text
d:/project/sample-marketplace-structure/
├── App.Web/                      # 🏢 THE MONOLITH HOST (User, Ordering)
│   ├── Program.cs                # Configures MassTransit (RabbitMQ), gRPC Clients
│   ├── Endpoints/                # Maps HTTP requests -> gRPC calls (BFF Pattern)
│   └── ...
│
├── App.Service.Catalog/          # 🚀 THE MICROSERVICE (Catalog)
│   ├── Program.cs                # Configures gRPC Server, Kestrel (HTTP/2)
│   └── Services/                 # gRPC Implementation (CatalogGrpcService.cs)
│
├── App.Modules.*/                # 🧩 THE MODULES (Business Logic)
│   ├── Features/                 # Vertical Slices (CreateProduct, generic handlers)
│   ├── Data/                     # EF Core DbContexts
│   └── ...
│
├── App.Shared.Protos/            # 📜 CONTRACTS
│   └── catalog.proto             # Shared gRPC definitions
│
├── App.Shared/                   # 🤝 SHARED KERNEL
│   ├── Events/                   # Integration Events (OrderCreated)
│   └── Infrastructure/           # Behaviors, Exceptions, Setup
│
├── docker-compose.yml            # 🐳 INFRASTRUCTURE (Postgres, RabbitMQ, Services)
└── manage-db.ps1                 # 🛠️ DB HELPER SCRIPT
```

---

## 🚀 Getting Started

### Prerequisites
-   [Docker Desktop](https://www.docker.com/products/docker-desktop)
-   [.NET 10 SDK](https://dotnet.microsoft.com/download)

### 1. Start Infrastructure & Services
Run everything (Database, Broker, API, Service) with one command:
```bash
docker-compose up -d --build
```

**Services Running:**
-   **App.Web**: `http://localhost:5000` (API & Swagger)
-   **App.Service.Catalog**: `http://localhost:5002` (gRPC)
-   **PostgreSQL**: `localhost:5433` (Port mapped to avoid 5432 conflicts)
-   **RabbitMQ**: `localhost:15672` (Management UI: guest/guest)

### 2. Verify It Works
Go to **Swagger**: [http://localhost:5000/swagger](http://localhost:5000/swagger)

1.  **Test RPC**: Call `GET /api/products`.
    -   *Behind the scenes*: `App.Web` -> (gRPC) -> `App.Service.Catalog` -> DB.
2.  **Test Events**: Call `POST /api/orders` (Simulated).
    -   *Behind the scenes*: `App.Web` publishes `OrderCreated` -> RabbitMQ -> `App.Service.Catalog` consumes it.

---

## 🛠️ Operational Guide

### Database Management
We have 3 DbContexts (`User`, `Ordering`, `Catalog`) running on the same Postgres instance.
Use the helper script to manage them:

```powershell
# Reset Everything (Drop DB, Re-Migrate, Seed)
./manage-db.ps1 -Action reset

# Add Migration for a specific module
./manage-db.ps1 -Action add -Name "NewField" -Module Catalog

# Update Database Schema
./manage-db.ps1 -Action update
```

**Connection Info (Local)**:
-   Host: `localhost`
-   Port: `5433` (Note the custom port!)
-   User/Pass: `postgres` / `postgres`
-   DB: `marketplace_db`

### Adding a New Service Interaction
1.  **Define Contract**: Add message/service in `App.Shared.Protos/your_service.proto`.
2.  **Implement Server**: Create `YourGrpcService.cs` in the Service project.
3.  **Register Client**: Add `AddGrpcClient` in `App.Web/Program.cs`.
4.  **Call It**: Inject `YourService.YourServiceClient` into your endpoint/handler.

---

## 🧰 Technology Stack

| Category | Technology | Usage |
| :--- | :--- | :--- |
| **Framework** | .NET 10 (ASP.NET Core) | Core platform |
| **Communication** | **gRPC** (Protobuf) | Inter-service (Sync) |
| **Messaging** | **MassTransit** + **RabbitMQ** | Event Bus (Async) |
| **Database** | **PostgreSQL** + **EF Core** | Persistence |
| **Logic** | **MediatR** | CQRS, Pipeline Behaviors |
| **Validation** | **FluentValidation** | Request validation |
| **UI** | **MudBlazor** | Admin Portal Components |

---

## 🛡️ Best Practices Implemented

1.  **Transactional Outbox**: We use MassTransit's Entity Framework Outbox.
    -   *Why?* Ensures that if we save to the DB but RabbitMQ is down, the event is saved in the DB and published later. Guarantees "At Least Once" delivery.
2.  **Structured Logging**: Explicit consumer logging.
3.  **Global Exception Handling**: Centralized middleware maps exceptions to standardized JSON responses.
4.  **Central Package Management**: `Directory.Packages.props` manages versions for the whole solution.