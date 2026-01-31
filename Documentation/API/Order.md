# Order API Documentation

---
## Overview
This controller contains all endpoints related to order management and checkout in the e-commerce system.

> **Note**: For general notes including response structure, error messages see [Notes.md](Notes.md)
---

## NOTES
### OrderStatus Enum
```csharp
public enum OrderStatus {
    Pending = 1,      // Order created 
    Processing = 2,   // order is being prepared
    Shipped = 3,      
    Delivered = 4,    
    Canceled = 5      
}
```

---


## 1. Checkout - Create New Order

### Endpoint
```http
POST /api/order/checkout
```

### Description
Create a new order from selected cart items. This endpoint deducts points, creates the order, and clears selected items from the cart.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `FullName` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Min Length**: 3 characters<br>- **Max Length**: 100 characters<br>- **Pattern**: Letters only (Arabic/English) `^[\u0600-\u06FFa-zA-Z\s]+$`<br>- **Valid Example**: "Ahmed Ali", "محمد علي"<br>- **Invalid Example**: "Ah", "Ahmed123" |
| `PhoneNumber` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Max Length**: 20 characters<br>- **Pattern**: Egyptian mobile number `^(010\|011\|012\|015)\d{8}$`<br>- **Valid Example**: "01012345678", "01112345678"<br>- **Invalid Example**: "123456", "02012345678" |
| `City` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Max Length**: 50 characters<br>- **Pattern**: Letters only (Arabic/English) `^[\u0600-\u06FFa-zA-Z\s]+$`<br>- **Valid Example**: "Cairo", "القاهرة"<br>- **Invalid Example**: "Cairo123" |
| `StreetAddress` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Min Length**: 5 characters<br>- **Max Length**: 200 characters<br>- **Valid Example**: "123 Main Street, Nasr City"<br>- **Invalid Example**: "St" (too short) |
| `BuildingNumber` | string | ❌ | - **Optional**<br>- **Max Length**: 20 characters (if provided)<br>- **Valid Example**: "Building 5", "عمارة 10" |
| `Landmark` | string | ❌ | - **Optional**<br>- **Max Length**: 100 characters (if provided)<br>- **Valid Example**: "Near Metro Station", "بجوار المسجد" |
| `ShippingNotes` | string | ❌ | - **Optional**<br>- **Max Length**: 500 characters (if provided)<br>- **Valid Example**: "Please call before delivery" |

### Request Example
```json
{
  "fullName": "Ahmed Mohamed",
  "phoneNumber": "01012345678",
  "city": "Cairo",
  "streetAddress": "15 El Nasr Street, Nasr City",
  "buildingNumber": "Building 7",
  "landmark": "Near City Center Mall",
  "shippingNotes": "Please call 30 minutes before delivery"
}
```

### Response Codes

#### ✅ 201 Created - Order Created Successfully
```json
{
  "statusCode": 201,
  "meta": null,
  "succeeded": true,
  "message": "Order placed successfully",
  "errors": null,
  "data": {
    "orderId": 1025,
    "status": 2,
    "statusName": "Processing",
    "totalPoints": 850,
    "orderDate": "2026-01-31T14:30:00Z",
    "itemsCount": 3
  }
}
```

#### ❌ 400 Bad Request - Invalid Data

**Validation Errors:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Full name is required",
    "Phone number is required",
    "City is required",
    "Street address is required"
  ],
  "data": null
}
```

**Specific Validation Error Examples:**
```json
// Full name too short
{
  "errors": ["Full name must be at least 3 characters"]
}

// Full name too long
{
  "errors": ["Full name cannot exceed 100 characters"]
}

// Full name with numbers
{
  "errors": ["Full name can only contain Arabic or English letters"]
}

// Invalid phone number
{
  "errors": ["Phone number must be a valid Egyptian mobile number (e.g., 01012345678)"]
}

// Phone number too long
{
  "errors": ["Phone number cannot exceed 20 characters"]
}

// City with numbers
{
  "errors": ["City name can only contain Arabic or English letters"]
}

// City too long
{
  "errors": ["City name cannot exceed 50 characters"]
}

// Street address too short
{
  "errors": ["Street address must be at least 5 characters"]
}

// Street address too long
{
  "errors": ["Street address cannot exceed 200 characters"]
}

// Building number too long
{
  "errors": ["Building number cannot exceed 20 characters"]
}

// Landmark too long
{
  "errors": ["Landmark cannot exceed 100 characters"]
}

// Shipping notes too long
{
  "errors": ["Shipping notes cannot exceed 500 characters"]
}
```

**Insufficient Points:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Insufficient points",
  "errors": [
    "You need 850 points but have only 500 points"
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
    "Product 'Eco-Friendly Water Bottle' has only 2 items in stock, requested 5"
  ],
  "data": null
}
```

#### ❌ 404 Not Found - Cart Empty or No Items Selected
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

**Or No Items Selected:**
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "No items selected for checkout",
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

## 2. Get My Orders - Get Current User's Orders (Paginated)

### Endpoint
```http
GET /api/order?PageNumber={pageNumber}&PageSize={pageSize}
```

### Description
Get a paginated list of current user's order history.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Query Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `PageNumber` | int? | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Default**: 1<br>- Requested page number |
| `PageSize` | int? | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Max Value**: Usually 100<br>- **Default**: 10<br>- Number of items per page |

### Response Codes

#### ✅ 200 OK - Orders Retrieved Successfully
```json
{
  "data": [
    {
      "id": 1025,
      "orderDate": "2026-01-31T14:30:00Z",
      "totalPoints": 850,
      "status": 2,
      "statusName": "Processing",
      "itemsCount": 3
    },
    {
      "id": 1020,
      "orderDate": "2026-01-28T10:15:00Z",
      "totalPoints": 450,
      "status": 4,
      "statusName": "Delivered",
      "itemsCount": 2
    },
    {
      "id": 1015,
      "orderDate": "2026-01-25T16:20:00Z",
      "totalPoints": 300,
      "status": 5,
      "statusName": "Canceled",
      "itemsCount": 1
    }
  ],
  "currentPage": 1,
  "totalPages": 3,
  "totalCount": 25,
  "meta": null,
  "pageSize": 10,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "messages": null,
  "succeeded": true
}
```

**Empty Page (No Orders):**
```json
{
  "data": [],
  "currentPage": 1,
  "totalPages": 0,
  "totalCount": 0,
  "meta": null,
  "pageSize": 10,
  "hasPreviousPage": false,
  "hasNextPage": false,
  "messages": null,
  "succeeded": true
}
```

#### ❌ 400 Bad Request - Invalid Parameters
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Invalid pagination parameters",
  "errors": [
    "PageNumber must be greater than 0",
    "PageSize must be between 1 and 100"
  ],
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

## 3. Get Order By ID - Get Order Details

### Endpoint
```http
GET /api/order/{id}
```

### Description
Get detailed information about a specific order including items and shipping details.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: `{id:int}` in route<br>- **Example**: 1025, 1020 |

### Response Codes

#### ✅ 200 OK - Order Found
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Order retrieved successfully",
  "errors": null,
  "data": {
    "id": 1025,
    "orderDate": "2026-01-31T14:30:00Z",
    "totalPoints": 850,
    "status": 2,
    "statusName": "Processing",
    "shippingDetails": {
      "fullName": "Ahmed Mohamed",
      "phoneNumber": "01012345678",
      "city": "Cairo",
      "streetAddress": "15 El Nasr Street, Nasr City",
      "buildingNumber": "Building 7",
      "landmark": "Near City Center Mall",
      "shippingNotes": "Please call 30 minutes before delivery"
    },
    "orderItems": [
      {
        "id": 2501,
        "productId": 45,
        "productName": "Eco-Friendly Water Bottle",
        "productDescription": "Reusable stainless steel water bottle",
        "productMainImageUrl": "https://storage.example.com/products/45/main.jpg",
        "quantity": 2,
        "unitPointsAtPurchase": 250,
        "totalPoints": 500
      },
      {
        "id": 2502,
        "productId": 46,
        "productName": "Organic Cotton Bag",
        "productDescription": "Reusable shopping bag made from 100% organic cotton",
        "productMainImageUrl": "https://storage.example.com/products/46/main.jpg",
        "quantity": 1,
        "unitPointsAtPurchase": 180,
        "totalPoints": 180
      },
      {
        "id": 2503,
        "productId": 50,
        "productName": "Bamboo Cutlery Set",
        "productDescription": "Eco-friendly reusable cutlery set",
        "productMainImageUrl": "https://storage.example.com/products/50/main.jpg",
        "quantity": 1,
        "unitPointsAtPurchase": 170,
        "totalPoints": 170
      }
    ]
  }
}
```

**Order with Minimal Shipping Details:**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Order retrieved successfully",
  "errors": null,
  "data": {
    "id": 1020,
    "orderDate": "2026-01-28T10:15:00Z",
    "totalPoints": 450,
    "status": 4,
    "statusName": "Delivered",
    "shippingDetails": {
      "fullName": "Sara Ali",
      "phoneNumber": "01112345678",
      "city": "Alexandria",
      "streetAddress": "10 Corniche Street",
      "buildingNumber": null,
      "landmark": null,
      "shippingNotes": null
    },
    "orderItems": [
      {
        "id": 2490,
        "productId": 48,
        "productName": "Solar Panel Kit",
        "productDescription": "Portable solar charger",
        "productMainImageUrl": "https://storage.example.com/products/48/main.jpg",
        "quantity": 1,
        "unitPointsAtPurchase": 450,
        "totalPoints": 450
      }
    ]
  }
}
```

#### ❌ 404 Not Found - Order Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Order not found",
  "errors": null,
  "data": null
}
```

**Or Order Belongs to Another User:**
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Order not found",
  "errors": null,
  "data": null
}
```

#### ❌ 400 Bad Request - Invalid ID
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Invalid order ID",
  "errors": [
    "The value 'abc' is not valid for id."
  ],
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

## 4. Cancel Order - Cancel Existing Order

### Endpoint
```http
PATCH /api/order/{id}/cancel
```

### Description
Cancel an existing order and refund points if they were already deducted. Only pending and processing orders can be canceled.

### Authentication
- **Required**: `User` role
- **Header**: `Authorization: Bearer {token}`

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: Order ID to cancel<br>- **Example**: 1025, 1020 |

### Response Codes

#### ✅ 200 OK - Order Canceled Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Order canceled successfully. 850 points have been refunded to your account.",
  "errors": null,
  "data": "Order canceled successfully. 850 points have been refunded to your account."
}
```

**Order Canceled (No Points Refunded - Was Pending):**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Order canceled successfully.",
  "errors": null,
  "data": "Order canceled successfully."
}
```

#### ❌ 400 Bad Request - Order Cannot Be Canceled

**Order Already Shipped:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Cannot cancel order",
  "errors": [
    "Order has already been shipped and cannot be canceled"
  ],
  "data": null
}
```

**Order Already Delivered:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Cannot cancel order",
  "errors": [
    "Order has already been delivered and cannot be canceled"
  ],
  "data": null
}
```

**Order Already Canceled:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Cannot cancel order",
  "errors": [
    "Order is already canceled"
  ],
  "data": null
}
```

#### ❌ 404 Not Found - Order Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Order not found",
  "errors": null,
  "data": null
}
```

**Or Order Belongs to Another User:**
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Order not found",
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
