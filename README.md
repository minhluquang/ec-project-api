## EC Project API

Production-ready .NET 9 Web API for an e-commerce backend. It covers core domains end-to-end: catalog (products, variants, categories), inventory, cart, orders, payments, discounts, shipping, reviews, returns, suppliers and purchase orders, user accounts with roles/permissions, and location/address data.

This README is tailored for recruiters and reviewers to quickly understand the scope, technology choices, architecture, and how to run/test the project locally.

---

## Tech Stack

- Runtime/Framework: .NET 9 (ASP.NET Core Web API)
- Database: SQL Server via Entity Framework Core 9
  - Lazy Loading Proxies, retry on transient failures, fluent configuration
- AuthN/AuthZ: JWT Bearer with custom middleware; roles & permissions
- API Docs: Swagger (Swashbuckle) + API Versioning
- Object Mapping: AutoMapper
- Crypto: BCrypt.Net for passwords
- Media: Cloudinary for product images
- Payment: Sepay QR integration (configurable)
- CORS: Frontend dev origin whitelisted (http://localhost:5173)

Key NuGet packages (see `ec-project-api/ec-project-api.csproj`):

- Microsoft.EntityFrameworkCore.SqlServer, .Design, .Tools, .Proxies
- Microsoft.AspNetCore.Authentication.JwtBearer
- Swashbuckle.AspNetCore
- AutoMapper + AutoMapper.Extensions.Microsoft.DependencyInjection
- BCrypt.Net-Next
- CloudinaryDotNet

---

## Architecture at a Glance

Request flow: Controllers → Facades → Services → Repositories → DataContext/DB

- Controllers: define routes, handle input, and standardize responses (via `Controllers/Base/BaseController.cs` + `Dtos/response/ResponseData.cs`).
- Facades: orchestrate cross-service business logic (e.g., `Facades/products/ProductFacade.cs` combines product details with reviews, sales, and related items).
- Services: core domain logic per bounded context (products, orders, payments, reviews, etc.).
- Repositories: data access abstraction (CRUD and queries).
- Data: `Data/DataContext.cs` configures all entities with indexes, relationships, precision, and constraints using EF Core Fluent API.

Infrastructure & middleware:

- `Security/JwtMiddleware.cs`: whitelists public paths (auth, swagger, health), validates token, attaches `ClaimsPrincipal` to `HttpContext.User`.
- `Security/CustomAuthenticationEntryPoint.cs` and `Security/CustomAccessDeniedHandler.cs`: consistent JSON for 401/403.
- Swagger with Bearer auth for easy testing; API versioning groups endpoints as `v1`.
- CORS policy `AllowFrontend` for local development.

Automatic DI registration: `RegisterServicesFromAssembly` in `Program.cs` scans interfaces/implementations and Facades to register them in the container.

---

## Project Structure

```
ec-project-api/
├─ ec-project-api/                  # Main project
│  ├─ Program.cs                    # App bootstrap: DI, DB, Auth, Swagger, CORS, pipeline
│  ├─ Data/
│  │  └─ DataContext.cs            # EF Core DbContext + Fluent configuration
│  ├─ Controller/                   # API controllers grouped by domain
│  │  ├─ products/                  # Product, Category, Color, Size, Variant, Image, Group
│  │  ├─ orders/, payment/, cart/   # Orders, payments, cart APIs
│  │  ├─ reviews/, product-return/
│  │  ├─ suppliers/, purchases/
│  │  ├─ provinces/, wards/, addresses/
│  │  ├─ system/, users/, auth/
│  │  └─ homepage/, dashboard/
│  ├─ Facades/                      # Orchestrate multi-service logic
│  ├─ Services/                     # Domain services (incl. jwt, inventory, etc.)
│  ├─ Repository/                   # Data access (base + per module)
│  ├─ Models/                       # EF entities with navigation & timestamps
│  ├─ Dtos/                         # Request/Response DTOs, pagination
│  ├─ Helpers/                      # AutoMapper profiles, email, URL builder, extensions
│  ├─ Security/                     # JwtMiddleware, 401/403 handlers, JwtService
│  ├─ Constants/                    # messages/ & variables/ (routes, entity, status)
│  ├─ Interfaces/                   # Contracts for services/repositories
│  ├─ Migrations/                   # EF migrations and snapshot
│  └─ Properties/launchSettings.json
├─ ec-project-api.sln
└─ README.md
```

Base routing constants live in `Constants/variables/Path.cs`, standardizing the base path `/ec-project/api/v1` and resource roots (products, orders, payments, etc.).

---

## Notable Implementation Details

1. Strong EF Core modeling (`DataContext.cs`)

- Unique indexes (slug, SKU, codes), composite keys (e.g., `RolePermission`), decimal precision for monetary fields.
- Intentional cascade/restrict/set-null behaviors for safe data lifecycle.
- Query performance: unique and filtered indexes where appropriate.
- Lazy loading proxies; clean pagination pattern with `QueryOptions<T>` in services.

2. Solid JWT security

- Token generation/validation implemented in `Services/jwt/JwtService.cs`.
- `JwtMiddleware` handles whitelist, parsing, and attaching claims to the request context.
- Consistent JSON for 401/403; Swagger security scheme wired for live testing.

3. Consistent response & error handling

- `BaseController.ExecuteAsync` centralizes try/catch and returns `ResponseData<T>` with uniform structure.
- AutoMapper profiles in `Helpers/MappingProfiles.cs` calculate derived fields (sale price, primary image, thumbnails, totals) so API responses are clean and ready for UI.

4. Roles/permissions and status system

- `Role` ↔ `Permission` many-to-many with `RolePermission` bridge.
- `Status` table normalizes states per `EntityType` (Product, Order, Payment, …) for consistent workflows.

5. External integrations

- Cloudinary: product images (primary flag, display order, alt text).
- Sepay: QR-based payment (keys provided via configuration; do not commit real secrets in production).

---

## Business Modules (high-level)

- Auth & Users: registration/login, JWT issuance/refresh, profile, role/permission assignment.
- Catalog: Category, Material, Color, Size, ProductGroup.
- Products: Product, ProductVariant (unique SKU, Size/Color), ProductImage (primary, order).
- Inventory: per-variant stock quantities.
- Cart: Cart + CartItem (unique per cart + variant), update/remove.
- Orders: Order + OrderItem, totals, shipping fee, discount application.
- Payments: PaymentMethod, PaymentDestination, Payment with statuses.
- Discounts: code management and linkage to orders.
- Reviews: user reviews per OrderItem, review images, reporting.
- Returns: return requests per OrderItem, replacement variant, processing statuses.
- Shipping: carriers (Ship).
- Suppliers & Purchase Orders: inbound procurement, items, profit percentages.
- Locations: Province, Ward, Address (linked to user and geo entities).
- Homepage & Dashboard: aggregated data for UI.

Each module typically follows: Controller → (Facade) → Service → Repository → Model/DTO. Example: `Controller/products/ProductController.cs` uses `ProductFacade` to return product detail enriched with ratings, sold count, and related products.

---

## API Surface (quick examples)

Base Path: `/ec-project/api/v1`

- Auth

  - POST `/auth/login` — issue access & refresh tokens
  - POST `/auth/refresh-token` — refresh tokens

- Products

  - GET `/products` — paged listing with basic filters (status, group, category, color, material)
  - GET `/products/{slug}` — detail + related + rating + sold
  - GET `/products/search?search=...` — top 5 suggestions for autosuggest
  - GET `/products/catelog?...` — paged listing by category slug with advanced filters (color/material/group/price/stock) and ordering
  - GET `/products/catelog/filter-options?categorySlug=...` — filter options for UI

- Orders

  - GET `/orders/{userId}` — user’s orders (auth required)
  - POST `/orders` — create order

- Payments

  - GET `/payment-methods` — available payment methods
  - POST `/payments` — record payment

- Locations & Address
  - GET `/provinces`, GET `/wards?provinceId=...`
  - CRUD `/addresses`

Use Swagger at `/swagger` to browse all endpoints and test with a Bearer token.

---

## Configuration & Secrets

App configuration is in `appsettings.json`/`appsettings.Development.json`:

- ConnectionStrings, Jwt (Secret, Issuer, Audience, ExpirationMinutes, RefreshExpirationDays), Cloudinary, Sepay, etc.

Recommendations:

- Do NOT commit real secrets. Use environment variables or dotnet user-secrets for local development.
- Provide an `appsettings.example.json` (todo) with placeholder keys for easier onboarding.

---

## Run Locally (Windows/PowerShell)

Prerequisites: .NET SDK 9, SQL Server running locally.

1. Update `ConnectionStrings:DefaultConnection` to your local SQL Server.

2. Apply migrations and run the API:

```powershell
# (Optional) Apply migrations if you use CLI tools
# dotnet tool install --global dotnet-ef
# dotnet ef database update --project .\ec-project-api\ec-project-api.csproj

# Run the API
dotnet run --project .\ec-project-api\ec-project-api.csproj
```

3. Open Swagger UI:

- http://localhost:<port>/swagger
- Base API path: http://localhost:<port>/ec-project/api/v1

4. Public endpoints to try without token:

- GET `/products?PageNumber=1&PageSize=10`
- GET `/products/search?search=shirt`

5. Auth-required endpoints:

- Obtain token via `/auth/login`, then click “Authorize” in Swagger and paste `Bearer <token>`.

---

## Code Quality Conveniences

- AutoMapper profiles (`Helpers/MappingProfiles.cs`) centralize mappings and computed fields.
- Route constants (`Constants/variables/Path.cs`) keep URLs consistent.
- `Controllers/Base/BaseController.cs` unifies responses and error handling.
- `Security/JwtMiddleware.cs` provides uniform JWT handling and respects `[AllowAnonymous]` where declared.

---

## Roadmap / Next Steps

- Add `appsettings.example.json` with placeholders (no secrets).
- Seed initial roles/permissions and statuses via migration.
- Health checks endpoint `/health` and Ops-friendly telemetry.
- Rate limiting and idempotency keys for payment/order endpoints.
- Unit/integration tests for critical services (Products, Orders, Payments).

---

## Contact

For a quick walkthrough or demo, feel free to reach out (contact info in CV). Happy to discuss trade-offs, design choices, and how this API integrates with the frontend.
