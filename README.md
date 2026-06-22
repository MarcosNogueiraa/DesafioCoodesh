# Developer Evaluation Project

`READ CAREFULLY`

## Use Case
**You are a developer on the DeveloperStore team. Now we need to implement the API prototypes.**

As we work with `DDD`, to reference entities from other domains, we use the `External Identities` pattern with denormalization of entity descriptions.

Therefore, you will write an API (complete CRUD) that handles sales records. The API needs to be able to inform:

* Sale number
* Date when the sale was made
* Customer
* Total sale amount
* Branch where the sale was made
* Products
* Quantities
* Unit prices
* Discounts
* Total amount for each item
* Cancelled/Not Cancelled

It's not mandatory, but it would be a differential to build code for publishing events of:
* SaleCreated
* SaleModified
* SaleCancelled
* ItemCancelled

If you write the code, **it's not required** to actually publish to any Message Broker. You can log a message in the application log or however you find most convenient.

### Business Rules

* Purchases above 4 identical items have a 10% discount
* Purchases between 10 and 20 identical items have a 20% discount
* It's not possible to sell above 20 identical items
* Purchases below 4 items cannot have a discount

These business rules define quantity-based discounting tiers and limitations:

1. Discount Tiers:
   - 4+ items: 10% discount
   - 10-20 items: 20% discount

2. Restrictions:
   - Maximum limit: 20 items per product
   - No discounts allowed for quantities below 4 items

## Overview
This section provides a high-level overview of the project and the various skills and competencies it aims to assess for developer candidates. 

See [Overview](/.doc/overview.md)

## Tech Stack
This section lists the key technologies used in the project, including the backend, testing, frontend, and database components. 

See [Tech Stack](/.doc/tech-stack.md)

## Frameworks
This section outlines the frameworks and libraries that are leveraged in the project to enhance development productivity and maintainability. 

See [Frameworks](/.doc/frameworks.md)

<!-- 
## API Structure
This section includes links to the detailed documentation for the different API resources:
- [API General](./docs/general-api.md)
- [Products API](/.doc/products-api.md)
- [Carts API](/.doc/carts-api.md)
- [Users API](/.doc/users-api.md)
- [Auth API](/.doc/auth-api.md)
-->

## Project Structure
This section describes the overall structure and organization of the project files and directories. 

See [Project Structure](/.doc/project-structure.md)

---

# Sales API — Implementation Notes

This section documents the **Sales** domain implemented on top of the provided
template (`template/backend`), following the same vertical-slice / DDD
architecture used by the existing `Users` domain.

## What was implemented

A complete CRUD for sales records plus the cancellation flows and domain events:

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/sales` | Create a sale (computes item discounts and total) |
| `GET` | `/api/sales/{id}` | Get a sale by id (with items) |
| `GET` | `/api/sales?_page=&_size=&_order=` | Paginated/ordered list of sales |
| `PUT` | `/api/sales/{id}` | Update a sale (rebuilds items, recomputes totals) |
| `DELETE` | `/api/sales/{id}` | Delete a sale |
| `POST` | `/api/sales/{id}/cancel` | Cancel the whole sale |
| `POST` | `/api/sales/{id}/items/{itemId}/cancel` | Cancel a single item |

**External Identities pattern**: Customer, Branch and Product are referenced by
their id plus a denormalized description (`CustomerName`, `BranchName`,
`ProductName`) stored on the sale/item.

**Domain events** (`SaleCreated`, `SaleModified`, `SaleCancelled`,
`ItemCancelled`) are modeled as MediatR `INotification`s and published from the
handlers. As allowed by the challenge, they are **not** sent to a real message
broker — dedicated notification handlers log them through the application log
(Serilog), e.g. `[SaleCreated] SaleId=... SaleNumber=... TotalAmount=...`.

## Business rules (discount tiers)

Discounts are calculated by the domain (`SaleItem.ApplyDiscountRules`) based on
the quantity of identical items:

| Quantity | Discount |
|---|---|
| `< 4` | 0% |
| `4 – 9` | 10% |
| `10 – 20` | 20% |
| `> 20` | **not allowed** (validation / domain error) |

> Note: the challenge text is ambiguous ("above 4" in prose vs. "4+" in the
> structured summary). This implementation follows the **structured summary**:
> 4 identical items already qualify for the 10% tier.

## Tech / patterns

- .NET 8, MediatR (CQRS), AutoMapper, FluentValidation (via the existing
  `ValidationBehavior` pipeline), EF Core + PostgreSQL.
- Layers touched: `Domain` (entities/validators/events/repository interface),
  `ORM` (mappings, `SaleRepository`, migration), `IoC` (DI registration),
  `Application` (handlers), `WebApi` (controller + request/response contracts).
- The `ValidationExceptionMiddleware` was extended to also translate
  `KeyNotFoundException` into a `404 Not Found` response.

## How to build & test

```bash
cd template/backend
dotnet build Ambev.DeveloperEvaluation.sln
dotnet test tests/Ambev.DeveloperEvaluation.Unit/Ambev.DeveloperEvaluation.Unit.csproj
```

Unit tests cover the discount tiers, the 20-item limit, total recalculation
(ignoring cancelled items) and every application handler (NSubstitute + Bogus +
FluentAssertions).

## Running against PostgreSQL (optional, end-to-end)

A migration `AddSales` is included. With a PostgreSQL instance configured in
`src/Ambev.DeveloperEvaluation.WebApi/appsettings.json` (`DefaultConnection`):

```bash
cd template/backend
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Then explore the endpoints via Swagger UI.

> The `AddSales` migration also adds the `CreatedAt`/`UpdatedAt` columns to the
> `Users` table: those properties already existed on the `User` entity but were
> missing from the original initial migration, so EF Core syncs them here.
