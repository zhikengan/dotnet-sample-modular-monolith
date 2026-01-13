# Modular Monolith Marketplace

This project is a practical example of a **Modular Monolith** architecture using **Vertical Slicing**. If those terms sound like buzzwords to you, don't worry! This guide is written specifically to explain the *what*, *why*, and *how* in simple terms.

---

## Core Concepts

Before diving into the code, let's understand the architectural decisions behind this project.

### 1. Modular Monolith
**Analogy:** Think of a typical Monolith as a **Studio Apartment**—everything (kitchen, bed, desk) is in one big room. It's easy to start but gets messy quickly.
A **Microservices** architecture is like having **separate houses** for your kitchen, bedroom, and office. Great for isolation, but expensive and complex to travel between them.

A **Modular Monolith** is the sweet spot: **A large house with separate, dedicated rooms.**
- You have one "house" (DEPLOYMENT unit).
- But you have distinct "rooms" (MODULES) like `Ordering`, `Catalog`, and `User`.
- The `Ordering` module focuses *only* on orders and doesn't tangle with `User` logic.

### 2. Vertical Slicing
**Analogy:** Think of a Layered Architecture (Controller -> Service -> Repository) as a **Layer Cake**. To eat a piece, you cut through all layers. If you want to change the "Strawberry" flavor (a feature), you have to touch the frosting, the sponge, and the cream (multiple files across the project).

**Vertical Slicing** is like a **Cupcake** or a slice of Pizza 🍕.
- Every feature (e.g., "Create Product") is self-contained.
- The API Endpoint, the Logic, the Database Query, and the Validation rule are all in **one folder**.
- **Benefit:** When you work on "Creating a Product", you don't need to hunt for files in 5 different places. You just open the `CreateProduct` folder.

### 3. Sync vs Async Communication
Modules need to talk to each other. For example, "When a User registers, send them a Welcome Email."

#### Synchronous (Sync) - "The Phone Call"
- **How it works:** You call someone, and you *wait* on the line until they answer.
- **When to use:** When you need an answer *right now* (e.g., Requesting a list of products to show on the screen).
- **In this Project:**
    - **Intra-Module (Inside):** We use **MediatR** (Commands/Queries).
    - **Inter-Module (Between):** We use **Contacts** (Shared Interfaces).
        - Example: `Ordering` needs to know if a User exists. It calls `IUserModuleFacade.UserExistsAsync()` which is defined in `App.Shared` and implemented by `User` module.

#### 📮 Asynchronous (Async) - "The Mail"
- **How it works:** You drop a letter in the mailbox and go back to work. The receiver picks it up whenever they are ready. You don't wait.
- **When to use:** For side effects or things that can happen later (e.g., "Order Placed" -> "Update Inventory" -> "Send Email").
- **In this Project:** We use **MassTransit** (a library) to send messages functionality.
    - **Event:** A module publishes an event (e.g., `UserRegistered`).
    - **Consumer:** Other modules subscribe to it. If `App.Modules.Notification` hears `UserRegistered`, it sends an email. The `User` module doesn't know or care about the email; it just announced "User Registered!".

---

## 📂 Project Structure

Here is how the project is organized to support these patterns:

### 1. The Host (`App.Web`)
This is the **Entry Point**. Think of it as the "Main Hallway" that connects all the rooms.
- **Responsibility:** It starts the application, connects to the database, and loads all the Modules.
- It doesn't contain business logic. It just says: "Hey `Catalog`, `Ordering`, and `User` modules, come join the party!"

### 2. The Modules (`App.Modules.*`)
These are the "Rooms". Each one is a separate project.
- **`App.Modules.Catalog`**: Handles Products, Categories.
- **`App.Modules.Ordering`**: Handles Orders, Carts.
- **`App.Modules.User`**: Handles Registration, Login.
- **Rule:** Modules **DO NOT** reference each other directly. `Ordering` cannot directly call code in `Catalog`. They must use "The Mail" (Events) or shared contracts.

### 3. The Shared Kernel (`App.Shared`)
This is the "Foundation".
- Contains things used by everyone:
    - **Integration Events**: The "letters" we mail context (e.g., `OrderCreated`).
    - **Common Behaviors**: Logic like "Validate every request" or "Log every error".

---

## �️ Admin Portal (Blazor)

This project includes a **Sample Admin Portal** built with **Blazor Server**.
It demonstrates how a UI can live inside the same Modular Monolith and interact with the modules.

### Interaction Model
Because the Admin Portal runs in the **Same Process** (`App.Web`) as the API, it does NOT need to make HTTP calls to fetch data.
Instead, it injects `MediatR` (ISender) and calls the Modules directly! 🤯
- **Efficient**: No network overhead.
- **Simple**: Uses the same Command/Query objects as the API.

### Key Pages
- **`/` (Dashboard)**: A simple dashboard showing mock statistics (Orders, Revenue).
    - Source: `App.Portal.Admin/Pages/Dashboard.razor`
- **`/products` (Product Management)**: A real working page to list and add products.
    - **Listing**: Calls `GetProductsQuery` in the `Catalog` module.
    - **Creating**: Calls `CreateProductCommand` using a Dialog.
    - Source: `App.Portal.Admin/Pages/Products.razor`

---

## 📂 Detailed Folder Structure

Here is a map to help you navigate the codebase.
Instead of listing every module (which looks the same), here is the **Typical Structure** of a module (e.g., `App.Modules.User`).

```text
d:/project/sample-marketplace-structure/
├── App.Web/                      # 🏁 THE HOST
│   ├── Program.cs                # Entry point, services registration
│   ├── appsettings.json          # Configuration
│   └── Components/               # Blazor Server Root
│
├── App.Modules.*/                # 🧩 TYPICAL MODULE (e.g., User, Catalog, Ordering)
│   ├── Features/                 # 🍰 VERTICAL SLICES (The Core Logic)
│   │   ├── RegisterUser/         # 👈 ONE FEATURE = ONE FOLDER
│   │   │   ├── RegisterUserEndpoint.cs   # API Endpoint (Receives HTTP request)
│   │   │   ├── RegisterUserCommand.cs    # Request DTO (Input data)
│   │   │   ├── RegisterUserHandler.cs    # Business Logic (Saves to DB)
│   │   │   └── RegisterUserValidator.cs  # Validation Rules (Check input)
│   │   └── ...
│   ├── EventHandlers/            # � ASYNC LISTENERS
│   │   └── OrderCreatedConsumer.cs # React to events from OTHER modules
│   ├── Data/                     # 💾 DATABASE
│   │   └── UserDbContext.cs      # Module-specific EF Core Context
│   ├── Models/                   # 🧱 DOMAIN MODELS
│   │   └── User.cs               # Entities specific to this module
│   ├── Migrations/               # 🗄️ DB MIGRATIONS
│   └── Module.cs                 # 🔌 MODULE SETUP (DI Registration)
│
├── App.Portal.Admin/             # 🖥️ ADMIN PORTAL (UI)
│   ├── Pages/                    # Blazor Pages (Dashboard.razor, Products.razor)
│   ├── Components/               # Reusable Widgets
│   └── _Imports.razor            # Global Usings
│
├── App.Shared/                   # 🤝 SHARED KERNEL (Used by everyone)
│   ├── Infrastructure/           # Base interfaces (ICommand, IQuery)
│   ├── Events/                   # Integration Events (Contracts)
│   ├── Contacts/                 # 📞 SHARED INTERFACES (Sync Communication)
│   │   └── IUserModuleFacade.cs  # Contract for User Module
│   └── Utils/                    # Common utilities
│
├── docker-compose.yml            # 🐳 Infrastructure (PostgreSQL)
└── README.md                     # 📖 This file!
```

---

## �🔍 A Look Inside a Feature (Vertical Slice)

Let's look at **`App.Modules.Catalog`** to see Vertical Slicing in action.
Location: `App.Modules.Catalog/Features/CreateProduct/`

In this ONE folder, you will find:
1.  **`CreateProductEndpoint.cs`**: The API Route (e.g., `POST /api/products`). It receives the HTTP request.
2.  **`CreateProductCommand.cs`**: A simple class holding the data (Name, Price, etc.).
3.  **`CreateProductHandler.cs`**: The Brains 🧠. It saves to the database.
4.  **`CreateProductValidator.cs`**: The Guard 🛡️. Checks if Price > 0.

Everything needed for "Create Product" is right there!

---

## 🚀 Getting Started

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for the Database)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Running the App
1.  **Start Infrastructure**:
    Open a terminal in the solution root and run:
    ```bash
    docker-compose up -d
    ```
    This starts the PostgreSQL database.

2.  **Run the Application**:
    ```bash
    dotnet run --project App.Web
    ```
    Or open the `.sln` file in Visual Studio/Rider and hit **Run** on `App.Web`.

3.  **Explore**:
    Go to `https://localhost:7139/swagger` (or the port shown in your terminal) to see the API endpoints.

---

## 🛡️ Error Handling & Standardization

We strictly enforce standardized responses and error handling to keep the API consistent.

### 1. Standardized Response (`StdResponse`)
Every API response is automatically wrapped in a `StdResponse<T>` structure.
You do **NOT** need to manually return `StdResponse`. Just return your data.

**Your Endpoint:**
```csharp
return new UserDto(Id: 1, Username: "admin");
```

**The API Output:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "username": "admin"
  }
}
```
*Note: If `success` is true, `errorCode` and `errorMessage` are hidden.*

### 2. Global Exception Handler
We use a `GlobalExceptionHandler` middleware to catch exceptions and convert them into standardized error responses.

- **`BusinessException`**: Thrown when a domain rule is violated (e.g. "Product out of stock").
  - Returns `400 Bad Request`.
  - Body: `{ "success": false, "errorCode": "OUT_OF_STOCK", "errorMessage": "..." }`
- **`UnauthorizedAccessException`**: Returns `401 Unauthorized`.
- **Other Exceptions**: Returns `500 Internal Server Error`.

### 3. Custom Assertions (`Assert`)
To check validation rules easily, we use a custom `Assert` utility in `App.Shared`.
It throws a `BusinessException` if the condition is not met.

```csharp
// Throws BusinessException if price is <= 0
Assert.True(price > 0, ErrorCodes.InvalidPrice);

// Throws BusinessException if product is null
Assert.NotNull(product, ErrorCodes.ProductNotFound);
```

### 4. Bypassing the Wrapper (`[SkipWrapping]`)
Sometimes you want to return a raw file, a stream, or a specific status code without the wrapper.
Use the `[SkipWrapping]` attribute on your endpoint.

```csharp
app.MapGet("/download", [SkipWrapping] () => 
{
    var fileBytes = File.ReadAllBytes("report.pdf");
    return Results.File(fileBytes, "application/pdf", "report.pdf");
});
```

### 5. Standardization via Middleware
You might wonder: *"How does every endpoint get the Response Wrapper and Authentication automatically?"*

It happens in the **Module Definition** when we map the endpoints.
Each module uses a helper extension `.WithAppDefaults()`:

```csharp
// In App.Modules.User/UserModule.cs
public static void MapUserModuleEndpoints(this IEndpointRouteBuilder app)
{
    var userGroup = app.MapGroup("api/users")
        .WithTags("Users")
        .WithAppDefaults(); // 👈 This adds Auth & ResponseWrapper!

    userGroup.MapRegisterUserEndpoint();
    // ...
}
```

This ensures you don't need to repeat `[Authorize]` or `Wrapper` logic in every single file.
If you need to opt-out, you use attributes like `[AllowAnonymous]` or `[SkipWrapping]`.

---

## 🛠️ Database Management

Because we have **multiple DbContexts** (one per module), managing migrations manually can be tedious.
We have included a PowerShell helper script: `manage-db.ps1`.

### Common Commands

1.  **Update Database**: Apply pending migrations to the database.
    ```powershell
    ./manage-db.ps1 -Action update
    ```

2.  **Add Migration**: Create a new migration file after changing your entities.
    > Tip: In a Modular Monolith, you usually work on one module at a time.
    > Use `-Module` to create a migration ONLY for that module.
    ```powershell
    # Update only the Ordering module
    ./manage-db.ps1 -Action add -Name "AddShippingAddress" -Module Ordering
    ```

3.  **Reset Database**: 🔥 **DANGER!** Drops the DB, deletes all migration files, creates a fresh Initial migration, and updates the DB.
    Use this when you made too many messy changes during development and want a clean slate.
    ```powershell
    ./manage-db.ps1 -Action reset
    ```

### 🧘 Independent Development

One of the biggest benefits of this architecture is **Isolation**.
When you are developing a feature in the **Ordering** module:
1.  You modify `Ordering` entities.
2.  You run `./manage-db.ps1 -Action add -Name "NewFeature" -Module Ordering`.
3.  You run `./manage-db.ps1 -Action update -Module Ordering`.

**You do NOT need to touch or worry about the User or Catalog modules.**
Your changes are isolated. You don't risk breaking the `Catalog` database schema while working on `Ordering`.

---

---

## 🧰 Technologies & Patterns

-   **MediatR**: Implementation of the Mediator pattern (Process Commands/Queries).
-   **MassTransit**: Message Bus for Async communication.
-   **FluentValidation**: Rules for validating data.
-   **PostgreSQL**: The main database.
-   **EF Core**: Database access.
-   **MudBlazor**: The UI Component library (if looking at UI pages).