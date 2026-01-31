# Product API Documentation

---
## Overview
This controller contains all endpoints related to product management in the e-commerce system.

> **Note**: For general notes including response structure, error messages see [Notes.md](Notes.md)
---


## 1. Add Product - Create New Product

### Endpoint
```http
POST /api/product/add
```

### Description
Add a new product to the e-commerce system with images.

### Authentication
- **Required**: `SuperAdmin` or `Admin`
- **Header**: `Authorization: Bearer {token}`

### Request Body (multipart/form-data)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `Name` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Max Length**: 50 characters<br>- **Valid Example**: "Eco-Friendly Water Bottle"<br>- **Invalid Example**: "" (empty), product name with more than 50 characters |
| `Description` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Max Length**: 200 characters<br>- **Valid Example**: "Reusable stainless steel water bottle"<br>- **Invalid Example**: "" (empty), description exceeding 200 characters |
| `PointsCost` | int | ✅ | - **Required**<br>- **Min Value**: Must be greater than 0<br>- **Valid Example**: 100, 500<br>- **Invalid Example**: 0, -10 |
| `Stock` | int | ✅ | - **Required**<br>- **Min Value**: Must be greater than or equal to 0<br>- **Valid Example**: 0, 50, 100<br>- **Invalid Example**: -5, -10 |
| `CategoryId` | int | ✅ | - **Required**<br>- **Min Value**: Must be greater than 0<br>- **Valid Example**: 1, 5<br>- **Invalid Example**: 0, -1 |
| `Images` | List<IFormFile> | ✅ | - **Required**<br>- **Count**: Exactly 3 images (1 main image + 2 additional images)<br>- **Each Image Max Size**: 5MB (5242880 bytes)<br>- **Allowed Types**: JPG, JPEG, PNG, GIF, WEBP<br>- **Error Message (Count)**: "Exactly 3 images are required (1 main image and 2 additional images)"<br>- **Error Message (Empty)**: "Each image file must not be empty"<br>- **Error Message (Type)**: "Only image files (jpg, jpeg, png, gif, webp) are allowed"<br>- **Error Message (Size)**: "Each image file size must not exceed 5MB" |

### Response Codes

#### ✅ 201 Created - Product Created Successfully
```json
{
  "statusCode": 201,
  "meta": null,
  "succeeded": true,
  "message": "Product created successfully",
  "errors": null,
  "data": {
    "id": 45,
    "name": "Eco-Friendly Water Bottle",
    "description": "Reusable stainless steel water bottle that helps reduce plastic waste",
    "pointsCost": 250,
    "stock": 100,
    "isActive": true,
    "createdAt": "2026-01-31T10:30:00Z",
    "category": {
      "id": 3,
      "name": "Eco Products"
    },
    "images": [
      {
        "id": 120,
        "url": "https://storage.example.com/products/45/main.jpg",
        "isMain": true
      },
      {
        "id": 121,
        "url": "https://storage.example.com/products/45/image1.jpg",
        "isMain": false
      },
      {
        "id": 122,
        "url": "https://storage.example.com/products/45/image2.jpg",
        "isMain": false
      }
    ]
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
    "Product name is required",
    "Product description is required",
    "Points cost must be greater than 0",
    "Stock must be greater than or equal to 0",
    "Category ID must be greater than 0",
    "Exactly 3 images are required (1 main image and 2 additional images)"
  ],
  "data": null
}
```

**Specific Validation Error Examples:**
```json
// Error in Name - empty
{
  "errors": ["Product name is required"]
}

// Error in Name - too long
{
  "errors": ["Product name cannot exceed 50 characters"]
}

// Error in Description - empty
{
  "errors": ["Product description is required"]
}

// Error in Description - too long
{
  "errors": ["Product description cannot exceed 200 characters"]
}

// Error in PointsCost - zero or negative
{
  "errors": ["Points cost must be greater than 0"]
}

// Error in Stock - negative
{
  "errors": ["Stock must be greater than or equal to 0"]
}

// Error in CategoryId - invalid
{
  "errors": ["Category ID must be greater than 0"]
}

// Error in Images - wrong count
{
  "errors": ["Exactly 3 images are required (1 main image and 2 additional images)"]
}

// Error in Images - empty file
{
  "errors": ["Each image file must not be empty"]
}

// Error in Images - invalid type
{
  "errors": ["Only image files (jpg, jpeg, png, gif, webp) are allowed"]
}

// Error in Images - size too large
{
  "errors": ["Each image file size must not exceed 5MB"]
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

## 2. Get All Products - Get All Products (Paginated)

### Endpoint
```http
GET /api/product?PageNumber={pageNumber}&PageSize={pageSize}&CategoryId={categoryId}
```

### Description
Get a list of products with pagination support and optional category filtering.

### Authentication
- **Not Required**: Public endpoint (anyone can access)

### Query Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `PageNumber` | int | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Default**: 1<br>- Requested page number |
| `PageSize` | int | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Max Value**: Usually 100<br>- **Default**: 10<br>- Number of items per page |
| `CategoryId` | int? | ❌ | - **Optional**<br>- If provided, filters products by category<br>- **Valid Example**: 1, 5<br>- If not provided, returns all products |

### Response Codes

#### ✅ 200 OK - Query Successful
```json
{
  "data": [
    {
      "id": 45,
      "name": "Eco-Friendly Water Bottle",
      "description": "Reusable stainless steel water bottle that helps reduce plastic waste",
      "pointsCost": 250,
      "stock": 100,
      "isActive": true,
      "createdAt": "2026-01-31T10:30:00Z",
      "category": {
        "id": 3,
        "name": "Eco Products"
      },
      "mainImageUrl": "https://storage.example.com/products/45/main.jpg"
    },
    {
      "id": 46,
      "name": "Organic Cotton Bag",
      "description": "Reusable shopping bag made from 100% organic cotton",
      "pointsCost": 150,
      "stock": 75,
      "isActive": true,
      "createdAt": "2026-01-30T09:15:00Z",
      "category": {
        "id": 3,
        "name": "Eco Products"
      },
      "mainImageUrl": "https://storage.example.com/products/46/main.jpg"
    }
  ],
  "currentPage": 1,
  "totalPages": 5,
  "totalCount": 50,
  "meta": null,
  "pageSize": 10,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "messages": null,
  "succeeded": true
}
```

**Empty Page (No Results):**
```json
{
  "data": [],
  "currentPage": 10,
  "totalPages": 5,
  "totalCount": 50,
  "meta": null,
  "pageSize": 10,
  "hasPreviousPage": true,
  "hasNextPage": false,
  "messages": null,
  "succeeded": true
}
```

**Filtered by Category:**
```json
{
  "data": [
    {
      "id": 47,
      "name": "Solar Powered Charger",
      "description": "Portable solar charger for mobile devices",
      "pointsCost": 500,
      "stock": 30,
      "isActive": true,
      "createdAt": "2026-01-29T14:20:00Z",
      "category": {
        "id": 5,
        "name": "Electronics"
      },
      "mainImageUrl": "https://storage.example.com/products/47/main.jpg"
    }
  ],
  "currentPage": 1,
  "totalPages": 1,
  "totalCount": 1,
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

---

## 3. Get Product By ID - Get Product By ID

### Endpoint
```http
GET /api/product/id/{Id}
```

### Description
Get details of a specific product using its ID, including all images.

### Authentication
- **Not Required**: Public endpoint (anyone can access)

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `Id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: `{Id:int}` in route<br>- **Example**: 1, 45, 100 |

### Response Codes

#### ✅ 200 OK - Product Found
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Product retrieved successfully",
  "errors": null,
  "data": {
    "id": 45,
    "name": "Eco-Friendly Water Bottle",
    "description": "Reusable stainless steel water bottle that helps reduce plastic waste",
    "pointsCost": 250,
    "stock": 100,
    "isActive": true,
    "createdAt": "2026-01-31T10:30:00Z",
    "category": {
      "id": 3,
      "name": "Eco Products"
    },
    "images": [
      {
        "id": 120,
        "url": "https://storage.example.com/products/45/main.jpg",
        "isMain": true
      },
      {
        "id": 121,
        "url": "https://storage.example.com/products/45/image1.jpg",
        "isMain": false
      },
      {
        "id": 122,
        "url": "https://storage.example.com/products/45/image2.jpg",
        "isMain": false
      }
    ]
  }
}
```

**Product with no category (if category was deleted):**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Product retrieved successfully",
  "errors": null,
  "data": {
    "id": 48,
    "name": "Bamboo Toothbrush Set",
    "description": "Set of 4 biodegradable bamboo toothbrushes",
    "pointsCost": 120,
    "stock": 200,
    "isActive": true,
    "createdAt": "2026-01-28T11:00:00Z",
    "category": null,
    "images": [
      {
        "id": 130,
        "url": "https://storage.example.com/products/48/main.jpg",
        "isMain": true
      },
      {
        "id": 131,
        "url": "https://storage.example.com/products/48/image1.jpg",
        "isMain": false
      },
      {
        "id": 132,
        "url": "https://storage.example.com/products/48/image2.jpg",
        "isMain": false
      }
    ]
  }
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

#### ❌ 400 Bad Request - Invalid ID
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Invalid product ID",
  "errors": [
    "The value 'abc' is not valid for Id."
  ],
  "data": null
}
```

---

## 4. Update Product - Update Existing Product

### Endpoint
```http
PATCH /api/product/{Id}
```

### Description
Update an existing product's information. All fields are optional except the ID.

### Authentication
- **Required**: `SuperAdmin` or `Admin`
- **Header**: `Authorization: Bearer {token}`

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `Id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: `{Id:int}` in route<br>- **Example**: 1, 45, 100 |

### Request Body (multipart/form-data)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `Name` | string | ❌ | - **Optional**<br>- If provided, cannot be empty<br>- **Max Length**: 50 characters<br>- **Valid Example**: "Updated Product Name"<br>- **Invalid Example**: "" (empty), name exceeding 50 characters |
| `Description` | string | ❌ | - **Optional**<br>- If provided, cannot be empty<br>- **Max Length**: 200 characters<br>- **Valid Example**: "Updated product description"<br>- **Invalid Example**: "" (empty), description exceeding 200 characters |
| `PointsCost` | int? | ❌ | - **Optional**<br>- If provided, must be greater than 0<br>- **Valid Example**: 100, 500<br>- **Invalid Example**: 0, -10 |
| `Stock` | int? | ❌ | - **Optional**<br>- If provided, must be greater than or equal to 0<br>- **Valid Example**: 0, 50, 100<br>- **Invalid Example**: -5, -10 |
| `IsActive` | bool? | ❌ | - **Optional**<br>- **Valid Example**: true, false<br>- Used to activate/deactivate product visibility |
| `CategoryId` | int? | ❌ | - **Optional**<br>- If provided, must be greater than 0<br>- **Valid Example**: 1, 5<br>- **Invalid Example**: 0, -1 |
| `Images` | List<IFormFile>? | ❌ | - **Optional**<br>- **If provided**: Must be exactly 3 images<br>- **Each Image Max Size**: 5MB (5242880 bytes)<br>- **Allowed Types**: JPG, JPEG, PNG, GIF, WEBP<br>- **Error Message (Count)**: "If updating images, exactly 3 images are required (1 main image and 2 additional images)"<br>- **Error Message (Empty)**: "Each image file must not be empty"<br>- **Error Message (Type)**: "Only image files (jpg, jpeg, png, gif, webp) are allowed"<br>- **Error Message (Size)**: "Each image file size must not exceed 5MB" |

**Important Note**: At least **one field** must be sent for update

### Request Example (Form Data)
```bash
PATCH /api/product/45
Content-Type: multipart/form-data
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

Name: Eco-Friendly Water Bottle - Premium Edition
PointsCost: 300
Stock: 150
IsActive: true
```

### Response Codes

#### ✅ 200 OK - Update Successful
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Product updated successfully",
  "errors": null,
  "data": {
    "id": 45,
    "name": "Eco-Friendly Water Bottle - Premium Edition",
    "description": "Reusable stainless steel water bottle that helps reduce plastic waste",
    "pointsCost": 300,
    "stock": 150,
    "isActive": true,
    "createdAt": "2026-01-31T10:30:00Z",
    "category": {
      "id": 3,
      "name": "Eco Products"
    },
    "images": [
      {
        "id": 120,
        "url": "https://storage.example.com/products/45/main.jpg",
        "isMain": true
      },
      {
        "id": 121,
        "url": "https://storage.example.com/products/45/image1.jpg",
        "isMain": false
      },
      {
        "id": 122,
        "url": "https://storage.example.com/products/45/image2.jpg",
        "isMain": false
      }
    ]
  }
}
```

#### ❌ 400 Bad Request - Invalid Data

**Name cannot be empty:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Product name cannot be empty"
  ],
  "data": null
}
```

**Name too long:**
```json
{
  "errors": ["Product name cannot exceed 50 characters"]
}
```

**Description cannot be empty:**
```json
{
  "errors": ["Product description cannot be empty"]
}
```

**Description too long:**
```json
{
  "errors": ["Product description cannot exceed 200 characters"]
}
```

**Invalid PointsCost:**
```json
{
  "errors": ["Points cost must be greater than 0"]
}
```

**Invalid Stock:**
```json
{
  "errors": ["Stock must be greater than or equal to 0"]
}
```

**Invalid CategoryId:**
```json
{
  "errors": ["Category ID must be greater than 0"]
}
```

**Wrong number of images:**
```json
{
  "errors": ["If updating images, exactly 3 images are required (1 main image and 2 additional images)"]
}
```

**Empty image file:**
```json
{
  "errors": ["Each image file must not be empty"]
}
```

**Invalid image type:**
```json
{
  "errors": ["Only image files (jpg, jpeg, png, gif, webp) are allowed"]
}
```

**Image size too large:**
```json
{
  "errors": ["Each image file size must not exceed 5MB"]
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

**Or Category Not Found (if CategoryId was provided but invalid):**
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

## 5. Delete Product - Delete Product and All Associated Images

### Endpoint
```http
DELETE /api/product/delete/{id}
```

### Description
Delete a product and all its associated images permanently from the system.

### Authentication
- **Required**: `SuperAdmin` or `Admin`
- **Header**: `Authorization: Bearer {token}`

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: `{id:int}` in route<br>- **Example**: 1, 45, 100 |

### Response Codes

#### ✅ 200 OK - Product Deleted Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Product deleted successfully",
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

#### ❌ 400 Bad Request - Invalid ID
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Invalid product ID",
  "errors": [
    "The value 'abc' is not valid for id."
  ],
  "data": null
}
```

**Or Delete Operation Failed:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Failed to delete product",
  "errors": [
    "Unable to delete product due to existing references"
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

## Usage Examples

### Example 1: Creating a New Product
```bash
POST /api/product/add
Content-Type: multipart/form-data
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

Name: Recycled Notebook
Description: A5 notebook made from 100% recycled paper
PointsCost: 80
Stock: 250
CategoryId: 2
Images: [file1.jpg, file2.jpg, file3.jpg]
```

### Example 2: Getting Products by Category
```bash
GET /api/product?PageNumber=1&PageSize=20&CategoryId=3
```

### Example 3: Updating Product Stock Only
```bash
PATCH /api/product/45
Content-Type: multipart/form-data
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

Stock: 50
```

### Example 4: Deactivating a Product
```bash
PATCH /api/product/45
Content-Type: multipart/form-data
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

IsActive: false
```

### Example 5: Updating Product with New Images
```bash
PATCH /api/product/45
Content-Type: multipart/form-data
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

Name: Updated Product Name
Images: [new_main.jpg, new_image1.jpg, new_image2.jpg]
```

---

## Important Notes

1. **Image Requirements**:
   - All products must have exactly **3 images**
   - First image is automatically set as the **main image** (isMain: true)
   - Other 2 images are additional images (isMain: false)
   - When updating images, you must provide all 3 images (old images will be replaced)

2. **Product Status**:
   - New products are **active by default** (isActive: true)
   - Use the `IsActive` field in update to activate/deactivate products
   - Inactive products may not appear in public listings (depends on business logic)

3. **Points System**:
   - `PointsCost` represents how many points users need to purchase this product
   - Must always be greater than 0

4. **Stock Management**:
   - Stock can be 0 (out of stock)
   - Stock cannot be negative
   - Update stock through the PATCH endpoint

5. **Category Association**:
   - Products must be associated with a valid category during creation
   - Category can be updated or removed later
   - If a category is deleted, the product's category field becomes null

6. **Permissions**:
   - Only **SuperAdmin** and **Admin** can create, update, or delete products
   - All users (including guests) can view products
   - Regular users cannot manage products

7. **Pagination**:
   - Default page size is 10 items
   - Maximum page size should not exceed 100 (enforced by business logic)
   - Use `CategoryId` parameter to filter products by category

---
