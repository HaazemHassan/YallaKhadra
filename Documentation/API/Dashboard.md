# DashboardController API Documentation

---
## Overview
This controller provides a dashboard analytics endpoint for admin users.

> **Note**: For general notes including response structure, error messages see [Notes.md](Notes.md)

---

## NOTES
### LeaderboardPeriod Enum
```csharp
public enum LeaderboardPeriod {
    AllTime = 0,
    Weekly = 1,
    Monthly = 2,
    Yearly = 3
}
```

---

## 1. Get Dashboard Analytics

### Endpoint
```http
GET /api/dashboard/analytics?Period={period}
```

### Description
Returns dashboard analytics data grouped into 3 main sections:
1. Reports Analytics
2. Users Overview
3. E-Commerce Analytics

### Authentication
- **Required**: `Admin` or `SuperAdmin`
- **Header**: `Authorization: Bearer {token}`

### Query Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `Period` | enum (`LeaderboardPeriod`) | ❌ | - Optional<br>- Default: `AllTime (0)`<br>- Allowed values: `AllTime (0)`, `Weekly (1)`, `Monthly (2)`, `Yearly (3)` |

### Response Structure

#### `ReportsAnalytics`
| Property | Type | Description |
|----------|------|-------------|
| `TotalReports` | int | Total number of waste reports |
| `PendingReports` | int | Number of pending reports |
| `InProgressReports` | int | Number of in-progress reports |
| `CompletedReports` | int | Number of completed reports |
| `WasteCollectedInKg` | decimal | Total collected waste weight in kilograms |
| `AiScans` | int | Total number of AI scans |

#### `UsersOverview`
| Property | Type | Description |
|----------|------|-------------|
| `TotalUsers` | int | Total number of users |
| `Workers` | int | Number of worker accounts |
| `Admins` | int | Number of admin and super admin accounts |

#### `ECommerceAnalytics`
| Property | Type | Description |
|----------|------|-------------|
| `Categories` | int | Total number of categories |
| `Products` | int | Total number of products |
| `Orders` | int | Total number of orders |
| `ItemsSold` | int | Total quantity of sold items |

### Response Codes

#### ✅ 200 OK - Analytics Retrieved Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Operation Succeeded",
  "errors": null,
  "data": {
    "reportsAnalytics": {
      "totalReports": 120,
      "pendingReports": 18,
      "inProgressReports": 22,
      "completedReports": 80,
      "wasteCollectedInKg": 1542.75,
      "aiScans": 95
    },
    "usersOverview": {
      "totalUsers": 340,
      "workers": 25,
      "admins": 4
    },
    "eCommerceAnalytics": {
      "categories": 12,
      "products": 48,
      "orders": 210,
      "itemsSold": 870
    }
  }
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

#### ❌ 403 Forbidden - Not Allowed
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

## Example Requests

### All Time
```http
GET /api/dashboard/analytics?Period=AllTime
```

### Weekly
```http
GET /api/dashboard/analytics?Period=Weekly
```

### Monthly
```http
GET /api/dashboard/analytics?Period=Monthly
```

### Yearly
```http
GET /api/dashboard/analytics?Period=Yearly
```
