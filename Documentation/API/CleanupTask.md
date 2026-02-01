# CleanupTaskController API Documentation

---
## Overview
This controller contains all endpoints related to cleanup task management, including assigning waste reports to workers and completing cleanup operations.

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
    Pending = 0,      // Report created, waiting for assignment
    InProgress = 1,   // Report assigned to worker, cleanup in progress
    Done = 2          // Cleanup completed
}
```

---

## 1. Assign Cleanup Task - Assign Waste Report to Worker

### Endpoint
```http
POST /api/cleanuptask/assign
```

### Description
Assign a pending waste report to the current worker for cleanup. The report status will be updated to "InProgress".

### Authentication
- **Required**: `Worker` role only
- **Header**: `Authorization: Bearer {token}`

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `ReportId` | int | ✅ | - **Required**<br>- **Min Value**: Greater than 0<br>- **Error Message**: "Report ID is required."<br>- **Valid Example**: 42<br>- **Invalid Example**: 0, -1 |

### Request Example
```json
{
  "reportId": 42
}
```

### Response Codes

#### ✅ 201 Created - Task Assigned Successfully
```json
{
  "statusCode": 201,
  "meta": null,
  "succeeded": true,
  "message": "Task assigned successfully",
  "errors": null,
  "data": null
}
```

#### ❌ 400 Bad Request - Invalid Data or Assignment Failed
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Report ID is required."
  ],
  "data": null
}
```

**Specific Error Examples:**
```json
// Report not found
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Report not found.",
  "errors": null,
  "data": null
}

// Report already assigned
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Report is already InProgress. Only pending reports can be assigned.",
  "errors": null,
  "data": null
}

// Report already completed
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Report is already Done. Only pending reports can be assigned.",
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

#### ❌ 404 Not Found - Report Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Report not found",
  "errors": null,
  "data": null
}
```

---

## 2. Complete Cleanup Task - Complete Assigned Cleanup Task

### Endpoint
```http
POST /api/cleanuptask/complete
```

### Description
Complete an assigned cleanup task by providing final waste details and uploading completion images. The report status will be updated to "Done" and points will be awarded to the original reporter.

### Authentication
- **Required**: `Worker` role only
- **Header**: `Authorization: Bearer {token}`

### Authorization
- Workers can **only complete tasks assigned to them**

### Request Body (multipart/form-data)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `TaskId` | int | ✅ | - **Required**<br>- **Min Value**: Greater than 0<br>- **Error Message**: "Task ID is required."<br>- **Valid Example**: 15<br>- **Invalid Example**: 0, -1 |
| `FinalWeightInKg` | decimal | ✅ | - **Required**<br>- **Min Value**: Greater than 0<br>- **Max Value**: 10,000 kg<br>- **Error Message (Min)**: "Weight must be greater than zero."<br>- **Error Message (Max)**: "Weight must not exceed 10,000 kg."<br>- **Valid Example**: 25.5, 150.0<br>- **Invalid Example**: 0, -5, 10001 |
| `FinalWasteType` | int | ✅ | - **Required**<br>- **Min Value**: Greater than or equal to 0<br>- **Valid Values**: 0-11 (See WasteType Enum)<br>- **Error Message**: "Waste type is required."<br>- **Valid Example**: 1 (Plastic), 2 (Paper), 5 (Organic) |
| `Images` | List<IFormFile> | ✅ | - **Required** (at least one image must be provided)<br>- **Min Count**: 1 image<br>- **Max Count**: 10 images per task<br>- **Allowed Types**: JPEG, JPG, PNG, GIF<br>- **Allowed Content-Type**: "image/jpeg", "image/jpg", "image/png", "image/gif"<br>- **Error Message (Required)**: "At least one image is required."<br>- **Error Message (Count)**: "Maximum 10 images allowed per task."<br>- **Error Message (Type)**: "Only image files (jpg, jpeg, png, gif) are allowed." |

### Response Codes

#### ✅ 200 OK - Task Completed Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Cleanup task completed successfully.",
  "errors": null,
  "data": null
}
```

#### ❌ 400 Bad Request - Invalid Data or Completion Failed
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Task ID is required.",
    "Weight must be greater than zero.",
    "Weight must not exceed 10,000 kg.",
    "Waste type is required.",
    "At least one image is required.",
    "Maximum 10 images allowed per task.",
    "Only image files (jpg, jpeg, png, gif) are allowed."
  ],
  "data": null
}
```

**Specific Validation Error Examples:**
```json
// Invalid Task ID
{
  "errors": ["Task ID is required."]
}

// Invalid Weight
{
  "errors": ["Weight must be greater than zero."]
}

// Weight exceeded
{
  "errors": ["Weight must not exceed 10,000 kg."]
}

// Invalid waste type
{
  "errors": ["Waste type is required."]
}

// Missing images
{
  "errors": ["At least one image is required."]
}

// Too many images
{
  "errors": ["Maximum 10 images allowed per task."]
}

// Invalid image type
{
  "errors": ["Only image files (jpg, jpeg, png, gif) are allowed."]
}
```

**Service-Level Error Examples:**
```json
// Task not found
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Cleanup task not found.",
  "errors": null,
  "data": null
}

// Task already completed
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Task is already completed.",
  "errors": null,
  "data": null
}

// Invalid waste type enum
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Invalid waste type.",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Not Authenticated or Not Assigned
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

**Not assigned to this task:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "You are not assigned to this task.",
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

#### ❌ 404 Not Found - Task Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "Task not found",
  "errors": null,
  "data": null
}
```

---

## 3. Get All Cleanup Tasks - Get All Cleanup Tasks (Paginated)

### Endpoint
```http
GET /api/cleanuptask?PageNumber={pageNumber}&PageSize={pageSize}
```

### Description
Get a list of all cleanup tasks with pagination support, sorted by assigned date (newest first). Includes full task details with report information, worker info, and images.

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
      "id": 15,
      "assignedAt": "2026-02-01T08:30:00Z",
      "completedAt": "2026-02-01T10:45:00Z",
      "finalWasteType": 1,
      "finalWasteTypeName": "Plastic",
      "finalWeightInKg": 25.5,
      "workerId": 10,
      "workerName": "Hassan Ahmed",
      "reportId": 42,
      "report": {
        "id": 42,
        "latitude": 30.0444,
        "longitude": 31.2357,
        "address": "Downtown Cairo",
        "status": 2,
        "wasteType": 1,
        "createdAt": "2026-01-31T14:20:00Z",
        "userId": 18,
        "userName": "Sara Ali",
        "images": [
          {
            "id": 101,
            "url": "https://storage.example.com/reports/42/waste.jpg",
            "uploadedAt": "2026-01-31T14:20:00Z"
          }
        ],
        "finalWasteType": 1,
        "finalWasteTypeName": "Plastic",
        "finalWeightInKg": 25.5,
        "cleanupImages": [
          {
            "id": 201,
            "url": "https://storage.example.com/cleanup/15/after1.jpg",
            "uploadedAt": "2026-02-01T10:45:00Z"
          }
        ]
      },
      "images": [
        {
          "id": 201,
          "url": "https://storage.example.com/cleanup/15/after1.jpg",
          "uploadedAt": "2026-02-01T10:45:00Z"
        },
        {
          "id": 202,
          "url": "https://storage.example.com/cleanup/15/after2.jpg",
          "uploadedAt": "2026-02-01T10:45:00Z"
        }
      ]
    },
    {
      "id": 14,
      "assignedAt": "2026-01-31T16:00:00Z",
      "completedAt": null,
      "finalWasteType": 0,
      "finalWasteTypeName": "Unknown",
      "finalWeightInKg": 0,
      "workerId": 12,
      "workerName": "Omar Mohamed",
      "reportId": 40,
      "report": {
        "id": 40,
        "latitude": 30.0626,
        "longitude": 31.2497,
        "address": "Nasr City",
        "status": 1,
        "wasteType": 3,
        "createdAt": "2026-01-31T10:30:00Z",
        "userId": 20,
        "userName": "Fatima Ali",
        "images": [],
        "finalWasteType": null,
        "finalWasteTypeName": null,
        "finalWeightInKg": null,
        "cleanupImages": []
      },
      "images": []
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

**Empty Page (No Results):**
```json
{
  "data": [],
  "currentPage": 5,
  "totalPages": 3,
  "totalCount": 25,
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

## 4. Get My Uncompleted Tasks - Get Worker's Uncompleted Tasks

### Endpoint
```http
GET /api/cleanuptask/my-uncompleted-tasks
```

### Description
Get the current worker's uncompleted cleanup tasks with full report details, ordered by assigned date (newest first).

### Authentication
- **Required**: `Worker` role only
- **Header**: `Authorization: Bearer {token}`

### Response Codes

#### ✅ 200 OK - Query Successful
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Retrieved 2 uncompleted tasks.",
  "errors": null,
  "data": [
    {
      "id": 16,
      "assignedAt": "2026-02-01T09:00:00Z",
      "completedAt": null,
      "finalWasteType": 0,
      "finalWasteTypeName": "Unknown",
      "finalWeightInKg": 0,
      "workerId": 10,
      "workerName": "Hassan Ahmed",
      "reportId": 45,
      "report": {
        "id": 45,
        "latitude": 30.0444,
        "longitude": 31.2357,
        "address": "Zamalek",
        "status": 1,
        "wasteType": 2,
        "createdAt": "2026-02-01T08:00:00Z",
        "userId": 22,
        "userName": "Laila Ibrahim",
        "images": [
          {
            "id": 110,
            "url": "https://storage.example.com/reports/45/paper.jpg",
            "uploadedAt": "2026-02-01T08:00:00Z"
          }
        ],
        "finalWasteType": null,
        "finalWasteTypeName": null,
        "finalWeightInKg": null,
        "cleanupImages": []
      },
      "images": []
    },
    {
      "id": 14,
      "assignedAt": "2026-01-31T16:00:00Z",
      "completedAt": null,
      "finalWasteType": 0,
      "finalWasteTypeName": "Unknown",
      "finalWeightInKg": 0,
      "workerId": 10,
      "workerName": "Hassan Ahmed",
      "reportId": 40,
      "report": {
        "id": 40,
        "latitude": 30.0626,
        "longitude": 31.2497,
        "address": "Nasr City",
        "status": 1,
        "wasteType": 3,
        "createdAt": "2026-01-31T10:30:00Z",
        "userId": 20,
        "userName": "Fatima Ali",
        "images": [
          {
            "id": 95,
            "url": "https://storage.example.com/reports/40/glass.jpg",
            "uploadedAt": "2026-01-31T10:30:00Z"
          }
        ],
        "finalWasteType": null,
        "finalWasteTypeName": null,
        "finalWeightInKg": null,
        "cleanupImages": []
      },
      "images": []
    }
  ]
}
```

**No Uncompleted Tasks:**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Retrieved 0 uncompleted tasks.",
  "errors": null,
  "data": []
}
```

#### ❌ 401 Unauthorized - Not Authenticated or Not Worker
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

**Not a worker:**
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Only workers can view cleanup tasks.",
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

## Additional Notes

### Task Lifecycle
1. **Pending Report**: User creates a waste report (status: Pending)
2. **Assignment**: Worker assigns the report to themselves (status: InProgress, cleanup task created)
3. **Completion**: Worker completes the task with final details and images (status: Done)
4. **Points Award**: Original reporter receives points based on waste type and weight

### Points Calculation
- Points are calculated using the `PointsWasteCalculatorHelper`
- Factors considered:
  - Waste type
  - Weight in kg
  - Accuracy (if reported waste type matches final waste type)
- Points are awarded to the original reporter, not the worker

### Image Upload Guidelines
- **Supported Formats**: JPEG, JPG, PNG, GIF
- **Maximum Images Per Task**: 10 images
- **Purpose**: Document the completed cleanup for verification

### Worker Authorization
- Workers can only complete tasks that are assigned to them
- Attempting to complete another worker's task will result in an authorization error
- Tasks can only be completed once

### Task Status Flow
1. **Uncompleted** (`CompletedAt` is null): Task is assigned but not yet completed
2. **Completed** (`CompletedAt` has value): Task is finished with final details

### Weight Guidelines
- **Minimum Weight**: Greater than 0 kg
- **Maximum Weight**: 10,000 kg (10 metric tons)
- **Format**: Decimal number (e.g., 25.5, 150.0)
- **Unit**: Kilograms (kg)

### Report Status Integration
- **Pending → InProgress**: When a worker assigns a report
- **InProgress → Done**: When a worker completes the cleanup task
- Only pending reports can be assigned
- Reports can only be completed through the completion endpoint

### Admin vs Worker Access
- **Admins/SuperAdmins**: Can view all cleanup tasks (read-only)
- **Workers**: Can assign reports, complete their own tasks, and view their uncompleted tasks
