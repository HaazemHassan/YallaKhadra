# WasteReportController API Documentation

---
## Overview
This controller contains all endpoints related to waste report management in the system.

> **Note**: For general notes including response structure, error messages see [Notes.md](Notes.md)
---

## NOTES
### WasteType Enum
```csharp
public enum WasteType {
    Unknown = 0,
    Plastic = 1,
    Paper = 2,
    Glass = 3,
    Metal = 4,
    Organic = 5,
    Electronic = 6,
    Construction = 9,
    Textile = 10,
    Mixed = 11
}
```

### ReportStatus Enum
```csharp
public enum ReportStatus {
    Pending = 0,
    InProgress = 1,
    Done = 2
}
```

---

## 1. Create Waste Report - Create New Waste Report

### Endpoint
```http
POST /api/wastereport/create
```

### Description
Create a new waste report with location details, waste type, and optional images.

### Authentication
- **Required**: `User` role only
- **Header**: `Authorization: Bearer {token}`

### Request Body (multipart/form-data)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `Latitude` | decimal | ✅ | - **Required**<br>- **Range**: Between -90 and 90<br>- **Example**: 30.0444<br>- **Invalid Example**: 95, -100 |
| `Longitude` | decimal | ✅ | - **Required**<br>- **Range**: Between -180 and 180<br>- **Example**: 31.2357<br>- **Invalid Example**: 200, -190 |
| `WasteType` | int | ✅ | - **Required**<br>- **Min Value**: Greater than 0<br>- **Valid Values**: 1-11 (See WasteType Enum)<br>- **Example**: 1 (Plastic), 2 (Paper) |
| `Address` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Max Length**: 500 characters<br>- **Valid Example**: "Downtown Cairo", "123 Main Street"<br>- **Error Message (Empty)**: "Address is required." |
| `Images` | List<IFormFile> | ✅ | - **Required** (at least one image must be provided)<br>- **Min Count**: 1 image<br>- **Max Count**: 10 images per report<br>- **Max Size Per Image**: 10MB (10485760 bytes)<br>- **Allowed Types**: JPEG, JPG, PNG, GIF<br>- **Allowed Content-Type**: "image/jpeg", "image/jpg", "image/png", "image/gif"<br>- **Error Message (Required)**: "At least one image is required."<br>- **Error Message (Count)**: "Maximum 10 images allowed per report."<br>- **Error Message (Size)**: "Each image must not exceed 10MB."<br>- **Error Message (Type)**: "Only image files (jpg, jpeg, png, gif) are allowed." |

### Response Codes

#### ✅ 201 Created - Waste Report Created Successfully
```json
{
  "statusCode": 201,
  "meta": null,
  "succeeded": true,
  "message": "Success",
  "errors": null,
  "data": {
    "id": 42,
    "latitude": 30.0444,
    "longitude": 31.2357,
    "address": "Downtown Cairo",
    "status": 0,
    "wasteType": 1,
    "createdAt": "2026-02-01T10:30:00Z",
    "userId": 15,
    "userName": "Ahmed Mohamed",
    "images": [
      {
        "id": 101,
        "url": "https://storage.example.com/reports/42/image1.jpg",
        "uploadedAt": "2026-02-01T10:30:00Z"
      },
      {
        "id": 102,
        "url": "https://storage.example.com/reports/42/image2.jpg",
        "uploadedAt": "2026-02-01T10:30:00Z"
      }
    ],
    "finalWasteType": null,
    "finalWasteTypeName": null,
    "finalWeightInKg": null,
    "cleanupImages": []
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
    "Latitude must be between -90 and 90.",
    "Longitude must be between -180 and 180.",
    "Waste type is required.",
    "Address is required.",
    "Address must not exceed 500 characters.",
    "At least one image is required.",
    "Maximum 10 images allowed per report.",
    "Each image must not exceed 10MB.",
    "Only image files (jpg, jpeg, png, gif) are allowed."
  ],
  "data": null
}
```

**Specific Validation Error Examples:**
```json
// Invalid Latitude
{
  "errors": ["Latitude must be between -90 and 90."]
}

// Invalid Longitude
{
  "errors": ["Longitude must be between -180 and 180."]
}

// Missing Waste Type
{
  "errors": ["Waste type is required."]
}

// Address too long
{
  "errors": ["Address must not exceed 500 characters."]
}

// Too many images
{
  "errors": ["Maximum 10 images allowed per report."]
}

// Image size exceeded
{
  "errors": ["Each image must not exceed 10MB."]
}

// Invalid image type
{
  "errors": ["Only image files (jpg, jpeg, png, gif) are allowed."]
}

// Missing address
{
  "errors": ["Address is required."]
}

// Missing images
{
  "errors": ["At least one image is required."]
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

## 2. Get All Waste Reports - Get All Waste Reports (Paginated)

### Endpoint
```http
GET /api/wastereport?PageNumber={pageNumber}&PageSize={pageSize}
```

### Description
Get a list of all waste reports with pagination support, sorted by creation date (newest first).

### Authentication
- **Required**: `SuperAdmin` or `Admin`
- **Header**: `Authorization: Bearer {token}`

### Query Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `PageNumber` | int | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Default**: 1<br>- Requested page number |
| `PageSize` | int | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Default**: 10<br>- Number of items per page |

### Response Codes

#### ✅ 200 OK - Query Successful
```json
{
  "data": [
    {
      "id": 42,
      "latitude": 30.0444,
      "longitude": 31.2357,
      "address": "Downtown Cairo",
      "status": 0,
      "wasteType": 1,
      "createdAt": "2026-02-01T10:30:00Z",
      "userId": 15,
      "userName": "Ahmed Mohamed",
      "images": [
        {
          "id": 101,
          "url": "https://storage.example.com/reports/42/image1.jpg",
          "uploadedAt": "2026-02-01T10:30:00Z"
        }
      ],
      "finalWasteType": null,
      "finalWasteTypeName": null,
      "finalWeightInKg": null,
      "cleanupImages": []
    },
    {
      "id": 41,
      "latitude": 31.2001,
      "longitude": 29.9187,
      "address": "Alexandria Corniche",
      "status": 2,
      "wasteType": 2,
      "createdAt": "2026-01-31T08:15:00Z",
      "userId": 18,
      "userName": "Sara Ali",
      "images": [],
      "finalWasteType": 2,
      "finalWasteTypeName": "Paper",
      "finalWeightInKg": 5.5,
      "cleanupImages": [
        {
          "id": 201,
          "url": "https://storage.example.com/cleanup/41/cleaned.jpg",
          "uploadedAt": "2026-01-31T12:00:00Z"
        }
      ]
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

## 3. Get Pending Waste Reports - Get Pending Reports (Paginated)

### Endpoint
```http
GET /api/wastereport/pending?PageNumber={pageNumber}&PageSize={pageSize}
```

### Description
Get a list of pending waste reports with pagination support, sorted by creation date (oldest first - FIFO queue).

### Authentication
- **Required**: `SuperAdmin`, `Admin`, or `Worker`
- **Header**: `Authorization: Bearer {token}`

### Query Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `PageNumber` | int | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Default**: 1<br>- Requested page number |
| `PageSize` | int | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Default**: 10<br>- Number of items per page |

### Response Codes

#### ✅ 200 OK - Query Successful
```json
{
  "data": [
    {
      "id": 35,
      "latitude": 30.0626,
      "longitude": 31.2497,
      "address": "Nasr City",
      "status": 0,
      "wasteType": 3,
      "createdAt": "2026-01-28T14:20:00Z",
      "userId": 12,
      "userName": "Omar Hassan",
      "images": [
        {
          "id": 85,
          "url": "https://storage.example.com/reports/35/glass.jpg",
          "uploadedAt": "2026-01-28T14:20:00Z"
        }
      ],
      "finalWasteType": null,
      "finalWasteTypeName": null,
      "finalWeightInKg": null,
      "cleanupImages": []
    },
    {
      "id": 36,
      "latitude": 30.0131,
      "longitude": 31.2089,
      "address": "Maadi",
      "status": 0,
      "wasteType": 4,
      "createdAt": "2026-01-29T09:45:00Z",
      "userId": 20,
      "userName": "Fatima Ali",
      "images": [],
      "finalWasteType": null,
      "finalWasteTypeName": null,
      "finalWeightInKg": null,
      "cleanupImages": []
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

#### ❌ 400 Bad Request - Invalid Parameters
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Invalid pagination parameters",
  "errors": [
    "PageNumber must be greater than 0"
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

#### ❌ 403 Forbidden - Insufficient Permissions
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

## 4. Get My Reports - Get Current User's Waste Reports

### Endpoint
```http
GET /api/wastereport/my?PageNumber={pageNumber}&PageSize={pageSize}&Status={status}
```

### Description
Get the authenticated user's waste reports with pagination and optional status filter. Returns only the first image per report (brief view).

### Authentication
- **Required**: User must be authenticated
- **Header**: `Authorization: Bearer {token}`

### Query Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `PageNumber` | int | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Default**: 1<br>- Requested page number |
| `PageSize` | int | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Default**: 10<br>- Number of items per page |
| `Status` | int? | ❌ | - **Optional**<br>- **Values**: 0 (Pending), 1 (InProgress), 2 (Done)<br>- **Default**: null (all statuses)<br>- Filter by report status |

### Response Codes

#### ✅ 200 OK - Query Successful
```json
{
  "data": [
    {
      "id": 42,
      "latitude": 30.0444,
      "longitude": 31.2357,
      "address": "Downtown Cairo",
      "status": 0,
      "statusName": "Pending",
      "wasteType": 1,
      "wasteTypeName": "Plastic",
      "createdAt": "2026-02-01T10:30:00Z",
      "userId": 15,
      "userName": "Ahmed Mohamed",
      "firstImage": {
        "id": 101,
        "url": "https://storage.example.com/reports/42/image1.jpg",
        "uploadedAt": "2026-02-01T10:30:00Z"
      }
    },
    {
      "id": 38,
      "latitude": 30.0444,
      "longitude": 31.2357,
      "address": "Zamalek",
      "status": 2,
      "statusName": "Done",
      "wasteType": 2,
      "wasteTypeName": "Paper",
      "createdAt": "2026-01-25T14:20:00Z",
      "userId": 15,
      "userName": "Ahmed Mohamed",
      "firstImage": null
    }
  ],
  "currentPage": 1,
  "totalPages": 2,
  "totalCount": 15,
  "meta": null,
  "pageSize": 10,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "messages": null,
  "succeeded": true
}
```

**With Status Filter (Status=0 - Pending Only):**
```json
{
  "data": [
    {
      "id": 42,
      "latitude": 30.0444,
      "longitude": 31.2357,
      "address": "Downtown Cairo",
      "status": 0,
      "statusName": "Pending",
      "wasteType": 1,
      "wasteTypeName": "Plastic",
      "createdAt": "2026-02-01T10:30:00Z",
      "userId": 15,
      "userName": "Ahmed Mohamed",
      "firstImage": {
        "id": 101,
        "url": "https://storage.example.com/reports/42/image1.jpg",
        "uploadedAt": "2026-02-01T10:30:00Z"
      }
    }
  ],
  "currentPage": 1,
  "totalPages": 1,
  "totalCount": 5,
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
    "PageNumber must be greater than 0"
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

---

## 5. Get Reports Near Location - Get Waste Reports Near Location

### Endpoint
```http
GET /api/wastereport/near?Latitude={latitude}&Longitude={longitude}&RadiusInKm={radius}
```

### Description
Get pending waste reports near a specific location within a given radius. Results are sorted by distance (nearest first).

### Authentication
- **Required**: `Worker` role only
- **Header**: `Authorization: Bearer {token}`

### Query Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `Latitude` | decimal | ✅ | - **Required**<br>- **Range**: Between -90 and 90<br>- **Example**: 30.0444 |
| `Longitude` | decimal | ✅ | - **Required**<br>- **Range**: Between -180 and 180<br>- **Example**: 31.2357 |
| `RadiusInKm` | double | ❌ | - **Optional**<br>- **Min Value**: Greater than 0<br>- **Max Value**: 100 km<br>- **Default**: 5.0 km<br>- Search radius in kilometers |

### Response Codes

#### ✅ 200 OK - Query Successful
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Found 3 reports within 5 km.",
  "errors": null,
  "data": [
    {
      "id": 42,
      "latitude": 30.0444,
      "longitude": 31.2357,
      "address": "Downtown Cairo",
      "status": 0,
      "wasteType": 1,
      "createdAt": "2026-02-01T10:30:00Z",
      "userId": 15,
      "userName": "Ahmed Mohamed",
      "images": [
        {
          "id": 101,
          "url": "https://storage.example.com/reports/42/image1.jpg",
          "uploadedAt": "2026-02-01T10:30:00Z"
        }
      ],
      "finalWasteType": null,
      "finalWasteTypeName": null,
      "finalWeightInKg": null,
      "cleanupImages": []
    },
    {
      "id": 40,
      "latitude": 30.0500,
      "longitude": 31.2400,
      "address": "Near Opera Square",
      "status": 0,
      "wasteType": 3,
      "createdAt": "2026-01-30T16:45:00Z",
      "userId": 22,
      "userName": "Laila Ibrahim",
      "images": [],
      "finalWasteType": null,
      "finalWasteTypeName": null,
      "finalWeightInKg": null,
      "cleanupImages": []
    }
  ]
}
```

**No Reports Found Within Radius:**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Found 0 reports within 10 km.",
  "errors": null,
  "data": []
}
```

#### ❌ 400 Bad Request - Invalid Parameters
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Latitude must be between -90 and 90.",
    "Longitude must be between -180 and 180.",
    "Radius must be greater than 0.",
    "Radius must not exceed 100 km."
  ],
  "data": null
}
```

**Specific Validation Error Examples:**
```json
// Invalid Latitude
{
  "errors": ["Latitude must be between -90 and 90."]
}

// Invalid Radius
{
  "errors": ["Radius must be greater than 0."]
}

// Radius too large
{
  "errors": ["Radius must not exceed 100 km."]
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

#### ❌ 403 Forbidden - Not Worker Role
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

## 6. Get Waste Report By ID - Get Waste Report By ID

### Endpoint
```http
GET /api/wastereport/{id}
```

### Description
Get details of a specific waste report using its ID. If the report status is "Done", includes cleanup task details.

### Authentication
- **Required**: User must be authenticated
- **Header**: `Authorization: Bearer {token}`

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: `{id:int}` in route<br>- **Example**: 1, 42, 100 |

### Response Codes

#### ✅ 200 OK - Report Found (Pending Status)
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Success",
  "errors": null,
  "data": {
    "id": 42,
    "latitude": 30.0444,
    "longitude": 31.2357,
    "address": "Downtown Cairo",
    "status": 0,
    "wasteType": 1,
    "createdAt": "2026-02-01T10:30:00Z",
    "userId": 15,
    "userName": "Ahmed Mohamed",
    "images": [
      {
        "id": 101,
        "url": "https://storage.example.com/reports/42/image1.jpg",
        "uploadedAt": "2026-02-01T10:30:00Z"
      },
      {
        "id": 102,
        "url": "https://storage.example.com/reports/42/image2.jpg",
        "uploadedAt": "2026-02-01T10:30:00Z"
      }
    ],
    "finalWasteType": null,
    "finalWasteTypeName": null,
    "finalWeightInKg": null,
    "cleanupImages": []
  }
}
```

**Report Found (Done Status - With Cleanup Details):**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Success",
  "errors": null,
  "data": {
    "id": 38,
    "latitude": 30.0444,
    "longitude": 31.2357,
    "address": "Zamalek",
    "status": 2,
    "wasteType": 2,
    "createdAt": "2026-01-25T14:20:00Z",
    "userId": 15,
    "userName": "Ahmed Mohamed",
    "images": [
      {
        "id": 90,
        "url": "https://storage.example.com/reports/38/paper.jpg",
        "uploadedAt": "2026-01-25T14:20:00Z"
      }
    ],
    "finalWasteType": 2,
    "finalWasteTypeName": "Paper",
    "finalWeightInKg": 8.5,
    "cleanupImages": [
      {
        "id": 205,
        "url": "https://storage.example.com/cleanup/38/after1.jpg",
        "uploadedAt": "2026-01-26T10:00:00Z"
      },
      {
        "id": 206,
        "url": "https://storage.example.com/cleanup/38/after2.jpg",
        "uploadedAt": "2026-01-26T10:01:00Z"
      }
    ]
  }
}
```

#### ❌ 404 Not Found - Report Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Waste report with ID 999 not found.",
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
  "message": "Invalid report ID",
  "errors": [
    "The value 'abc' is not valid for Id."
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

---

## Additional Notes

### Image Upload Guidelines
- **Supported Formats**: JPEG, JPG, PNG, GIF
- **Maximum Size Per Image**: 10MB
- **Maximum Images Per Report**: 10 images
- Images are uploaded to cloud storage and URLs are returned in the response

### Report Status Flow
1. **Pending** (0): Report created, waiting for assignment
2. **InProgress** (1): Report assigned to a worker
3. **Done** (2): Cleanup completed, includes final weight and cleanup images

### Waste Type Reference
- **Plastic** (1): Plastic bottles, bags, containers
- **Paper** (2): Cardboard, newspapers, office paper
- **Glass** (3): Glass bottles, jars
- **Metal** (4): Aluminum cans, metal scraps
- **Organic** (5): Food waste, garden waste
- **Electronic** (6): E-waste, old electronics
- **Construction** (9): Building materials, rubble
- **Textile** (10): Clothes, fabrics
- **Mixed** (11): Mixed/unsorted waste

### Location Data
- **Latitude/Longitude**: Uses decimal degrees format
- **Precision**: Up to 6 decimal places (9,6 precision)
- **Distance Calculation**: Uses Haversine formula for accurate distance measurement

### Pagination
- Default page size: 10 items
- Maximum page size: typically 100 items
- Pages are 1-indexed (first page is 1, not 0)
