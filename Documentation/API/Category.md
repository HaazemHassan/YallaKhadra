# Category API Documentation

---
## Overview
This controller contains all endpoints related to category management in the e-commerce system.

> **Note**: For general notes including response structure, error messages see [Notes.md](Notes.md)
---


## 1. Get Category By ID - Get Category Details by ID

### Endpoint
```http
GET /api/category/{id}
```

### Description
Get details of a specific category using its ID.

### Authentication
- **Not Required**: Public endpoint (anyone can access)

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: `{id:int}` in route<br>- **Example**: 1, 5, 10 |

### Response Codes

#### ✅ 200 OK - Category Found
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Category retrieved successfully",
  "errors": null,
  "data": {
    "id": 3,
    "name": "Eco Products",
    "description": "Environmentally friendly and sustainable products"
  }
}
```

#### ❌ 404 Not Found - Category Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Category not found",
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
  "message": "Invalid category ID",
  "errors": [
    "The value 'abc' is not valid for id."
  ],
  "data": null
}
```

---

## 2. Get Category By Name - Get Category Details by Name

### Endpoint
```http
GET /api/category/name/{name}
```

### Description
Get details of a specific category using its name.

### Authentication
- **Not Required**: Public endpoint (anyone can access)

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `name` | string | ✅ | - **Required**<br>- **Type**: String<br>- **Example**: "Eco Products", "Electronics" |

### Response Codes

#### ✅ 200 OK - Category Found
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Category retrieved successfully",
  "errors": null,
  "data": {
    "id": 3,
    "name": "Eco Products",
    "description": "Environmentally friendly and sustainable products"
  }
}
```

#### ❌ 404 Not Found - Category Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Category not found",
  "errors": null,
  "data": null
}
```

#### ❌ 400 Bad Request - Invalid Request
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Invalid request",
  "errors": [
    "Category name cannot be empty"
  ],
  "data": null
}
```

---

## 3. Add Category - Create New Category

### Endpoint
```http
POST /api/category
```

### Description
Create a new category in the system.

### Authentication
- **Required**: `SuperAdmin` or `Admin`
- **Header**: `Authorization: Bearer {token}`

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `Name` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Max Length**: 50 characters<br>- **Valid Example**: "Eco Products", "Electronics"<br>- **Invalid Example**: "" (empty), name exceeding 50 characters |
| `Description` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Max Length**: 200 characters<br>- **Valid Example**: "Environmentally friendly products"<br>- **Invalid Example**: "" (empty), description exceeding 200 characters |

### Request Example
```json
{
  "name": "Eco Products",
  "description": "Environmentally friendly and sustainable products for a greener lifestyle"
}
```

### Response Codes

#### ✅ 201 Created - Category Created Successfully
```json
{
  "statusCode": 201,
  "meta": null,
  "succeeded": true,
  "message": "Category created successfully",
  "errors": null,
  "data": {
    "id": 8,
    "name": "Eco Products",
    "description": "Environmentally friendly and sustainable products for a greener lifestyle"
  }
}
```

#### ❌ 400 Bad Request - Invalid Data
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Category name is required",
    "Category description is required"
  ],
  "data": null
}
```

**Specific Validation Error Examples:**
```json
// Name is empty
{
  "errors": ["Category name is required"]
}

// Name too long
{
  "errors": ["Category name cannot exceed 50 characters"]
}

// Description is empty
{
  "errors": ["Category description is required"]
}

// Description too long
{
  "errors": ["Category description cannot exceed 200 characters"]
}
```

#### ❌ 409 Conflict - Category Already Exists
```json
{
  "statusCode": 409,
  "meta": null,
  "succeeded": false,
  "message": "Category already exists",
  "errors": [
    "A category with the name 'Eco Products' already exists"
  ],
  "data": null
}
```

#### ❌ 401 Unauthorized - Missing Token
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

#### ❌ 403 Forbidden - Not Admin
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

## 4. Update Category - Update Existing Category

### Endpoint
```http
PATCH /api/category/{id}
```

### Description
Update an existing category's information. At least one field (Name or Description) must be provided.

### Authentication
- **Required**: `SuperAdmin` or `Admin`
- **Header**: `Authorization: Bearer {token}`

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: `{id:int}` in route<br>- **Example**: 1, 5, 10 |

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `Name` | string | ❌ | - **Optional**<br>- If provided, cannot be empty<br>- **Max Length**: 50 characters<br>- **Valid Example**: "Updated Category Name"<br>- **Invalid Example**: "" (empty), name exceeding 50 characters |
| `Description` | string | ❌ | - **Optional**<br>- If provided, cannot be empty<br>- **Max Length**: 200 characters<br>- **Valid Example**: "Updated category description"<br>- **Invalid Example**: "" (empty), description exceeding 200 characters |

**Important Note**: At least **one field** (Name or Description) must be sent for update

### Request Example
```json
{
  "name": "Eco-Friendly Products",
  "description": "Sustainable and environmentally friendly products"
}
```

**Update Only Name:**
```json
{
  "name": "Green Products"
}
```

**Update Only Description:**
```json
{
  "description": "Products that help protect our environment"
}
```

### Response Codes

#### ✅ 200 OK - Update Successful
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Category updated successfully",
  "errors": null,
  "data": {
    "id": 3,
    "name": "Eco-Friendly Products",
    "description": "Sustainable and environmentally friendly products"
  }
}
```

#### ❌ 400 Bad Request - Invalid Data

**No fields provided for update:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "At least one field (Name or Description) must be provided for update"
  ],
  "data": null
}
```

**Name cannot be empty:**
```json
{
  "errors": ["Category name cannot be empty"]
}
```

**Name too long:**
```json
{
  "errors": ["Category name cannot exceed 50 characters"]
}
```

**Description cannot be empty:**
```json
{
  "errors": ["Category description cannot be empty"]
}
```

**Description too long:**
```json
{
  "errors": ["Category description cannot exceed 200 characters"]
}
```

#### ❌ 404 Not Found - Category Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Category not found",
  "errors": null,
  "data": null
}
```

#### ❌ 409 Conflict - Category Name Already Exists
```json
{
  "statusCode": 409,
  "meta": null,
  "succeeded": false,
  "message": "Category name already exists",
  "errors": [
    "Another category with the name 'Electronics' already exists"
  ],
  "data": null
}
```

#### ❌ 401 Unauthorized - Missing Token
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

#### ❌ 403 Forbidden - Not Admin
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

## 5. Delete Category - Delete Category

### Endpoint
```http
DELETE /api/category/{id}
```

### Description
Delete a category from the system. Categories with associated products cannot be deleted.

### Authentication
- **Required**: `SuperAdmin` or `Admin`
- **Header**: `Authorization: Bearer {token}`

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: `{id:int}` in route<br>- **Example**: 1, 5, 10 |

### Response Codes

#### ✅ 200 OK - Category Deleted Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Category deleted successfully",
  "errors": null,
  "data": null
}
```

#### ❌ 404 Not Found - Category Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Category not found",
  "errors": null,
  "data": null
}
```

#### ❌ 400 Bad Request - Cannot Delete Category with Products
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Cannot delete category",
  "errors": [
    "Cannot delete category because it has associated products"
  ],
  "data": null
}
```

**Or:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Cannot delete category",
  "errors": [
    "This category contains 15 products. Please remove or reassign products before deleting the category."
  ],
  "data": null
}
```

#### ❌ 401 Unauthorized - Missing Token
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

#### ❌ 403 Forbidden - Not Admin
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
