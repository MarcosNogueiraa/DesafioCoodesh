[Back to README](../README.md)

### Sales

The Sales resource handles sale records. Customer, Branch and Product are
referenced through the **External Identities** pattern (denormalized id +
description), and quantity-based discounts are applied automatically by the
domain.

All responses are wrapped in the standard API envelope:

```json
{
  "success": true,
  "message": "string",
  "data": { }
}
```

Discount rules (applied per item, based on quantity of identical items):

| Quantity | Discount |
|---|---|
| `< 4` | 0% |
| `4 – 9` | 10% |
| `10 – 20` | 20% |
| `> 20` | not allowed (validation error) |

---

#### POST /api/sales
- Description: Create a new sale. Item discounts and totals are calculated server-side.
- Request Body:
  ```json
  {
    "saleNumber": "string",
    "saleDate": "2024-01-15T10:00:00Z",
    "customerId": "guid",
    "customerName": "string",
    "branchId": "guid",
    "branchName": "string",
    "items": [
      {
        "productId": "guid",
        "productName": "string",
        "quantity": "integer (1-20)",
        "unitPrice": "decimal"
      }
    ]
  }
  ```
- Response: `201 Created`
  ```json
  {
    "success": true,
    "message": "Sale created successfully",
    "data": {
      "id": "guid",
      "saleNumber": "string",
      "saleDate": "2024-01-15T10:00:00Z",
      "customerId": "guid",
      "customerName": "string",
      "branchId": "guid",
      "branchName": "string",
      "totalAmount": "decimal",
      "isCancelled": false,
      "createdAt": "2024-01-15T10:00:00Z",
      "updatedAt": null,
      "items": [
        {
          "id": "guid",
          "productId": "guid",
          "productName": "string",
          "quantity": "integer",
          "unitPrice": "decimal",
          "discount": "decimal",
          "totalAmount": "decimal",
          "isCancelled": false
        }
      ]
    }
  }
  ```
- Errors: `400 Bad Request` (validation, e.g. quantity above 20 or duplicate sale number).

#### GET /api/sales
- Description: Retrieve a paginated list of sales.
- Query Parameters:
  - `_page` (optional): Page number (default: 1)
  - `_size` (optional): Items per page (default: 10)
  - `_order` (optional): Ordering, e.g. `"saleDate desc, saleNumber asc"`
- Response: `200 OK`
  ```json
  {
    "success": true,
    "data": [ { "id": "guid", "saleNumber": "string", "...": "see sale object above" } ],
    "totalItems": "integer",
    "currentPage": "integer",
    "totalPages": "integer"
  }
  ```

#### GET /api/sales/{id}
- Description: Retrieve a specific sale (with its items) by id.
- Path Parameters:
  - `id`: Sale id (guid)
- Response: `200 OK` — same sale object as the create response.
- Errors: `404 Not Found` when the sale does not exist.

#### PUT /api/sales/{id}
- Description: Update a sale. Items are replaced and discounts/totals recalculated.
- Path Parameters:
  - `id`: Sale id (guid)
- Request Body: same shape as `POST /api/sales` (without `id`).
- Response: `200 OK` — updated sale object.
- Errors: `400 Bad Request` (validation), `404 Not Found`.

#### DELETE /api/sales/{id}
- Description: Delete a sale.
- Path Parameters:
  - `id`: Sale id (guid)
- Response: `200 OK`
  ```json
  { "success": true, "message": "Sale deleted successfully" }
  ```
- Errors: `404 Not Found`.

#### POST /api/sales/{id}/cancel
- Description: Cancel an entire sale (sets `isCancelled = true`). Raises a `SaleCancelled` event.
- Path Parameters:
  - `id`: Sale id (guid)
- Response: `200 OK`
  ```json
  { "success": true, "message": "Sale cancelled successfully" }
  ```
- Errors: `404 Not Found`.

#### POST /api/sales/{id}/items/{itemId}/cancel
- Description: Cancel a single item of a sale and recalculate the sale total. Raises an `ItemCancelled` event.
- Path Parameters:
  - `id`: Sale id (guid)
  - `itemId`: Sale item id (guid)
- Response: `200 OK`
  ```json
  { "success": true, "message": "Sale item cancelled successfully" }
  ```
- Errors: `404 Not Found` (sale or item not found).

---

### Domain events

The following events are published by the application handlers and, as allowed
by the challenge, **logged** to the application log (Serilog) instead of being
sent to a real message broker:

- `SaleCreated` — after a sale is created
- `SaleModified` — after a sale is updated
- `SaleCancelled` — after a sale is cancelled
- `ItemCancelled` — after a single item is cancelled

<br>
<div style="display: flex; justify-content: space-between;">
  <a href="./users-api.md">Previous: Users API</a>
  <a href="./general-api.md">Back: General API</a>
</div>
