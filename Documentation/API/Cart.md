# Cart API Documentation

---
## Overview
This controller contains all endpoints related to shopping cart management in the e-commerce system.

> **Note**: For general notes including response structure, error messages see [Notes.md](Notes.md)
---


### Authentication Requirement
- **All cart endpoints require authentication with `User` role**
- Users can only access and manage their own cart
- **Header**: `Authorization: Bearer {token}`

---


## 1. Get Cart - Get Current User's Shopping Cart

### Endpoint
```http
GET /api/cart
```

### Description
Retrieve the current user's shopping cart with all items, total points, and price change information.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Response Codes

#### ✅ 200 OK - Cart Retrieved Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cart retrieved successfully",
  "errors": null,
  "data": {
    "cartId": 25,
    "userId": 15,
    "items": [
      {
        "id": 101,
        "productId": 45,
        "productName": "Eco-Friendly Water Bottle",
        "productDescription": "Reusable stainless steel water bottle",
        "productMainImageUrl": "https://storage.example.com/products/45/main.jpg",
        "quantity": 2,
        "pointsCost": 250,
        "currentProductPointsCost": 250,
        "priceChanged": false,
        "totalPoints": 500,
        "totalPointsWithCurrentPrice": 500,
        "isSelected": true,
        "addedAt": "2026-01-30T10:30:00Z"
      },
      {
        "id": 102,
        "productId": 46,
        "productName": "Organic Cotton Bag",
        "productDescription": "Reusable shopping bag made from 100% organic cotton",
        "productMainImageUrl": "https://storage.example.com/products/46/main.jpg",
        "quantity": 1,
        "pointsCost": 150,
        "currentProductPointsCost": 180,
        "priceChanged": true,
        "totalPoints": 150,
        "totalPointsWithCurrentPrice": 180,
        "isSelected": true,
        "addedAt": "2026-01-29T14:20:00Z"
      }
    ],
    "totalPoints": 650,
    "totalPointsWithCurrentPrices": 680,
    "hasPriceChanges": true,
    "createdAt": "2026-01-25T09:00:00Z",
    "updatedAt": "2026-01-30T10:30:00Z"
  }
}
```

**Empty Cart:**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cart retrieved successfully",
  "errors": null,
  "data": {
    "cartId": 30,
    "userId": 20,
    "items": [],
    "totalPoints": 0,
    "totalPointsWithCurrentPrices": 0,
    "hasPriceChanges": false,
    "createdAt": "2026-01-31T08:00:00Z",
    "updatedAt": "2026-01-31T08:00:00Z"
  }
}
```

**Cart with Price Changes:**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cart retrieved successfully",
  "errors": null,
  "data": {
    "cartId": 28,
    "userId": 18,
    "items": [
      {
        "id": 105,
        "productId": 50,
        "productName": "Solar Panel Kit",
        "productDescription": "Portable solar panel for outdoor use",
        "productMainImageUrl": "https://storage.example.com/products/50/main.jpg",
        "quantity": 1,
        "pointsCost": 1000,
        "currentProductPointsCost": 900,
        "priceChanged": true,
        "totalPoints": 1000,
        "totalPointsWithCurrentPrice": 900,
        "isSelected": true,
        "addedAt": "2026-01-20T11:00:00Z"
      }
    ],
    "totalPoints": 1000,
    "totalPointsWithCurrentPrices": 900,
    "hasPriceChanges": true,
    "createdAt": "2026-01-20T11:00:00Z",
    "updatedAt": "2026-01-20T11:00:00Z"
  }
}
```

#### ❌ 404 Not Found - Cart Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Cart not found",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Not Authenticated
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Unauthorized",
  "errors": null,
  "data": null
}
```

#### ❌ 403 Forbidden - Not User Role
```json
{
  "statusCode": 403,
  "meta": null,
  "succeeded": false,
  "message": "Access denied. Insufficient permissions.",
  "errors": null,
  "data": null
}
```

---

## 2. Add to Cart - Add Product to Shopping Cart

### Endpoint
```http
POST /api/cart/items
```

### Description
Add a product to the shopping cart or update quantity if the product already exists in the cart.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `ProductId` | int | ✅ | - **Required**<br>- **Min Value**: Must be greater than 0<br>- **Valid Example**: 1, 45<br>- **Invalid Example**: 0, -1 |
| `Quantity` | int | ✅ | - **Required**<br>- **Min Value**: 1 (at least 1 item)<br>- **Max Value**: 100 (cannot exceed 100)<br>- **Default**: 1<br>- **Valid Example**: 1, 5, 50<br>- **Invalid Example**: 0, -5, 150 |

### Request Example
```json
{
  "productId": 45,
  "quantity": 2
}
```

### Response Codes

#### ✅ 201 Created - Product Added to Cart
```json
{
  "statusCode": 201,
  "meta": null,
  "succeeded": true,
  "message": "Product added to cart successfully",
  "errors": null,
  "data": {
    "cartItemId": 110,
    "productId": 45,
    "productName": "Eco-Friendly Water Bottle",
    "quantity": 2,
    "pointsCost": 250,
    "totalPoints": 500
  }
}
```

#### ✅ 200 OK - Product Quantity Updated (Product Already in Cart)
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cart item quantity updated successfully",
  "errors": null,
  "data": {
    "cartItemId": 101,
    "productId": 45,
    "productName": "Eco-Friendly Water Bottle",
    "quantity": 4,
    "pointsCost": 250,
    "totalPoints": 1000
  }
}
```

#### ❌ 400 Bad Request - Invalid Data

**Invalid ProductId:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Product ID must be greater than 0"
  ],
  "data": null
}
```

**Invalid Quantity (less than 1):**
```json
{
  "errors": ["Quantity must be at least 1"]
}
```

**Invalid Quantity (exceeds 100):**
```json
{
  "errors": ["Quantity cannot exceed 100"]
}
```

**Insufficient Stock:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Insufficient stock",
  "errors": [
    "Only 5 items available in stock"
  ],
  "data": null
}
```

**Product Not Active:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Product is not available",
  "errors": null,
  "data": null
}
```

#### ❌ 404 Not Found - Product Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Product not found",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Not Authenticated
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Unauthorized",
  "errors": null,
  "data": null
}
```

#### ❌ 403 Forbidden - Not User Role
```json
{
  "statusCode": 403,
  "meta": null,
  "succeeded": false,
  "message": "Access denied. Insufficient permissions.",
  "errors": null,
  "data": null
}
```

---

## 3. Update Cart Item Quantity - Update Quantity of Cart Item

### Endpoint
```http
PATCH /api/cart/items/{id}/quantity
```

### Description
Update the quantity of an existing item in the shopping cart.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: Cart item ID<br>- **Example**: 101, 105 |

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `Quantity` | int | ✅ | - **Required**<br>- **Min Value**: Must be greater than 0<br>- **Valid Example**: 1, 5, 10<br>- **Invalid Example**: 0, -5 |

### Request Example
```json
{
  "quantity": 5
}
```

### Response Codes

#### ✅ 200 OK - Quantity Updated Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cart item quantity updated successfully",
  "errors": null,
  "data": {
    "cartItemId": 101,
    "quantity": 5,
    "pointsCost": 250,
    "totalPoints": 1250
  }
}
```

#### ❌ 400 Bad Request - Invalid Data

**Invalid Quantity (zero or negative):**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Quantity must be greater than 0"
  ],
  "data": null
}
```

**Insufficient Stock:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Insufficient stock",
  "errors": [
    "Only 3 items available in stock"
  ],
  "data": null
}
```

#### ❌ 404 Not Found - Cart Item Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Cart item not found",
  "errors": null,
  "data": null
}
```

**Or User Trying to Update Another User's Cart Item:**
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Cart item not found",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Not Authenticated
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Unauthorized",
  "errors": null,
  "data": null
}
```

#### ❌ 403 Forbidden - Not User Role
```json
{
  "statusCode": 403,
  "meta": null,
  "succeeded": false,
  "message": "Access denied. Insufficient permissions.",
  "errors": null,
  "data": null
}
```

---

## 4. Toggle Cart Item Selection - Toggle Selection Status

### Endpoint
```http
PATCH /api/cart/items/{id}/selection
```

### Description
Toggle the selection status of a cart item. This is useful for selecting which items to checkout.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: Cart item ID<br>- **Example**: 101, 105 |

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `IsSelected` | bool | ✅ | - **Required**<br>- **Valid Values**: true, false<br>- **Example**: true (select item), false (unselect item) |

### Request Example
```json
{
  "isSelected": true
}
```

### Response Codes

#### ✅ 200 OK - Selection Updated Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cart item selection updated successfully",
  "errors": null,
  "data": {
    "cartItemId": 101,
    "isSelected": true
  }
}
```

**Unselecting an Item:**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cart item selection updated successfully",
  "errors": null,
  "data": {
    "cartItemId": 102,
    "isSelected": false
  }
}
```

#### ❌ 404 Not Found - Cart Item Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Cart item not found",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Not Authenticated
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Unauthorized",
  "errors": null,
  "data": null
}
```

#### ❌ 403 Forbidden - Not User Role
```json
{
  "statusCode": 403,
  "meta": null,
  "succeeded": false,
  "message": "Access denied. Insufficient permissions.",
  "errors": null,
  "data": null
}
```

---

## 5. Remove from Cart - Remove Item from Cart

### Endpoint
```http
DELETE /api/cart/items/{id}
```

### Description
Remove a specific item from the shopping cart.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: Cart item ID to remove<br>- **Example**: 101, 105 |

### Response Codes

#### ✅ 200 OK - Item Removed Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Item removed from cart successfully",
  "errors": null,
  "data": null
}
```

#### ❌ 404 Not Found - Cart Item Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Cart item not found",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Not Authenticated
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Unauthorized",
  "errors": null,
  "data": null
}
```

#### ❌ 403 Forbidden - Not User Role
```json
{
  "statusCode": 403,
  "meta": null,
  "succeeded": false,
  "message": "Access denied. Insufficient permissions.",
  "errors": null,
  "data": null
}
```

---

## 6. Clear Cart - Remove All Items from Cart

### Endpoint
```http
DELETE /api/cart/clear
```

### Description
Remove all items from the shopping cart at once.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Response Codes

#### ✅ 200 OK - Cart Cleared Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cart cleared successfully",
  "errors": null,
  "data": null
}
```

#### ❌ 404 Not Found - Cart Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Cart not found",
  "errors": null,
  "data": null
}
```

**Or Cart is Already Empty:**
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Cart is already empty",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Not Authenticated
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Unauthorized",
  "errors": null,
  "data": null
}
```

#### ❌ 403 Forbidden - Not User Role
```json
{
  "statusCode": 403,
  "meta": null,
  "succeeded": false,
  "message": "Access denied. Insufficient permissions.",
  "errors": null,
  "data": null
}
```

---

## 7. Sync Cart Prices - Sync Prices with Current Product Prices

### Endpoint
```http
POST /api/cart/sync-prices
```

### Description
Synchronize all cart items prices with the current product prices. This is useful when product prices have changed and you want to update the cart to reflect current pricing.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Response Codes

#### ✅ 200 OK - Prices Synced Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cart prices synced successfully. 3 items updated.",
  "errors": null,
  "data": null
}
```

**When No Prices Changed:**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cart prices are already up to date. 0 items updated.",
  "errors": null,
  "data": null
}
```

#### ❌ 404 Not Found - Cart Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Cart not found",
  "errors": null,
  "data": null
}
```

**Or Cart is Empty:**
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Cart is empty",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Not Authenticated
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Unauthorized",
  "errors": null,
  "data": null
}
```

#### ❌ 403 Forbidden - Not User Role
```json
{
  "statusCode": 403,
  "meta": null,
  "succeeded": false,
  "message": "Access denied. Insufficient permissions.",
  "errors": null,
  "data": null
}
```