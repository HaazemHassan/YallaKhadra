# UserController API Documentation

---
## Overview
This controller contains all endpoints related to user management in the system.

> **Note**: For general notes including response structure, error messages see [Notes.md](Notes.md)
---

## NOTES
### UserRole Enum
```csharp
public enum UserRole {
    SuperAdmin = 0,  
    Admin = 1,       
    Worker = 2,      
    User = 3         
}
```

---


## 1. Register - Register New User

### Endpoint
```http
POST /register
```

### Description
Register a new user account in the system.

### Authentication
- **Anonymous Only** - User must not be authenticated (for new users only)

### Request Body (multipart/form-data)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `FirstName` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Min Length**: 3 characters<br>- **Max Length**: 15 characters<br>- **Pattern**: Letters only (Arabic/English) `^[\p{L}]+$`<br>- **Valid Example**: "Ahmed", "محمد"<br>- **Invalid Example**: "Ah", "Ahmed123" |
| `LastName` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Min Length**: 3 characters<br>- **Max Length**: 15 characters<br>- **Pattern**: Letters only (Arabic/English) `^[\p{L}]+$`<br>- **Valid Example**: "Ali", "علي"<br>- **Invalid Example**: "A2", "Al" |
| `Email` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Max Length**: 35 characters<br>- **Format**: Valid email<br>- **Pattern**: Must contain valid domain `@.+\..+`<br>- **Valid Example**: "ahmed@example.com"<br>- **Invalid Example**: "ahmed@", "ahmed.com" |
| `Password` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Min Length**: 3 characters<br>- **Valid Example**: "Pass123"<br>- **Invalid Example**: "ab" |
| `ConfirmPassword` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Must Match**: Password<br>- **Error Message**: "Password does not match" |
| `Address` | string | ❌ | - **Optional**<br>- If provided, no specific constraints |
| `PhoneNumber` | string | ❌ | - **Optional**<br>- **Pattern**: `^\+?[0-9]\d{1,14}$`<br>- **Format**: Valid international number (2-15 digits)<br>- **Valid Example**: "+201234567890", "01234567890"<br>- **Invalid Example**: "123", "abc123" |
| `ProfileImage` | IFormFile | ❌ | - **Optional**<br>- **Max Size**: 5MB (5242880 bytes)<br>- **Allowed Types**: JPEG, JPG, PNG<br>- **Allowed Content-Type**: "image/jpeg", "image/jpg", "image/png"<br>- **Error Message (Size)**: "Profile image size must not exceed 5MB"<br>- **Error Message (Type)**: "Profile image must be a valid image file (JPEG, JPG, or PNG)" |

### Response Codes

#### ✅ 201 Created - Registration Successful
```json
{
  "statusCode": 201,
  "meta": null,
  "succeeded": true,
  "message": "User registered successfully",
  "errors": null,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": {
      "token": "a1b2c3d4e5f6g7h8i9j0",
      "userId": 15,
      "expirationDate": "2026-02-28T10:30:00Z",
      "createdAt": "2026-01-30T10:30:00Z"
    },
    "user": {
      "id": 15,
      "email": "ahmed.ali@example.com",
      "firstName": "Ahmed",
      "lastName": "Ali",
      "pointsBalance": 0,
      "profileImage": {
        "id": 42,
        "url": "https://storage.example.com/profiles/15/profile.jpg"
      },
      "address": "Cairo, Egypt",
      "phoneNumber": "+201234567890"
    }
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
    "FirstName can't be empty",
    "Email is not a valid email address",
    "Password must be at least of length 3",
    "Password does not match",
    "Phone number is not valid.",
    "Profile image size must not exceed 5MB"
  ],
  "data": null
}
```

**Specific Validation Error Examples:**
```json
// Error in FirstName - less than 3 characters
{
  "errors": ["The length of 'First Name' must be at least 3 characters. You entered 2 characters."]
}

// Error in FirstName - contains numbers
{
  "errors": ["First name must contain only letters."]
}

// Error in Email - invalid format
{
  "errors": ["Invalid email address format."]
}

// Error in Image - large size
{
  "errors": ["Profile image size must not exceed 5MB"]
}

// Error in Image - unsupported type
{
  "errors": ["Profile image must be a valid image file (JPEG, JPG, or PNG)"]
}
```

#### ❌ 409 Conflict - User Already Exists
```json
{
  "statusCode": 409,
  "meta": null,
  "succeeded": false,
  "message": "User already exists",
  "errors": [
    "Email is already registered"
  ],
  "data": null
}
```

**Or:**
```json
{
  "errors": ["Phone number is already in use"]
}
```

#### ❌ 403 Forbidden - User Already Authenticated
```json
{
  "statusCode": 403,
  "meta": null,
  "succeeded": false,
  "message": "Access denied. This endpoint is for anonymous users only.",
  "errors": null,
  "data": null
}
```

---

## 2. Get All Users - Get All Users (Paginated)

### Endpoint
```http
GET /api/user?PageNumber={pageNumber}&PageSize={pageSize}
```

### Description
Get a list of users with pagination support.

### Authentication
- **Required**: `SuperAdmin` or `Admin`
- **Header**: `Authorization: Bearer {token}`

### Query Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `PageNumber` | int? | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Default**: 1<br>- Requested page number |
| `PageSize` | int? | ❌ | - **Optional**<br>- **Min Value**: 1<br>- **Max Value**: Usually 100<br>- **Default**: 10<br>- Number of items per page |

### Response Codes

#### ✅ 200 OK - Query Successful
```json
{
  "data": [
    {
      "id": 1,
      "email": "ahmed@example.com",
      "firstName": "Ahmed",
      "lastName": "Mohamed",
      "pointsBalance": 150,
      "profileImage": {
        "id": 5,
        "url": "https://storage.example.com/profiles/1/profile.jpg"
      },
      "address": "Cairo, Egypt",
      "phone": "+201234567890"
    },
    {
      "id": 2,
      "email": "sara@example.com",
      "firstName": "Sara",
      "lastName": "Ali",
      "pointsBalance": 300,
      "profileImage": null,
      "address": "Alexandria, Egypt",
      "phone": "+201098765432"
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

## 3. Get User By ID - Get User By ID

### Endpoint
```http
GET /api/user/{id}
```

### Description
Get details of a specific user using their ID.

### Authentication
- **Required**: User must be authenticated
- **Header**: `Authorization: Bearer {token}`

### Authorization
- Users can **only view their own data** (when `id` matches their user ID)
- Admin/SuperAdmin can **view any user's data**

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: `{id:int}` in route<br>- **Example**: 1, 25, 100 |

### Response Codes

#### ✅ 200 OK - User Found
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "User retrieved successfully",
  "errors": null,
  "data": {
    "id": 15,
    "email": "ahmed@example.com",
    "firstName": "Ahmed",
    "lastName": "Mohamed",
    "pointsBalance": 250,
    "profileImage": {
      "id": 8,
      "url": "https://storage.example.com/profiles/15/profile.jpg"
    },
    "address": "123 Main St, Cairo",
    "phoneNumber": "+201234567890"
  }
}
```

**User without profile image:**
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "User retrieved successfully",
  "errors": null,
  "data": {
    "id": 20,
    "email": "sara@example.com",
    "firstName": "Sara",
    "lastName": "Ali",
    "pointsBalance": 100,
    "profileImage": null,
    "address": null,
    "phoneNumber": null
  }
}
```

#### ❌ 404 Not Found - User Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "User not found",
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
  "message": "Invalid user ID",
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

#### ❌ 403 Forbidden - Not Authorized to View This User
```json
{
  "statusCode": 403,
  "meta": null,
  "succeeded": false,
  "message": "Forbidden",
  "errors": null,
  "data": null
}
```

---

## 4. Check Email Availability - Check Email Availability

### Endpoint
```http
GET /api/user/check-email/{Email}
```

### Description
Check if an email address is available for registration.

### Authentication
- No requirements

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `Email` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Max Length**: 100 characters<br>- **Format**: Valid Email<br>- **Valid Example**: "test@example.com"<br>- **Invalid Example**: "test", "test@" |

### Response Codes

#### ✅ 200 OK - Email Available
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Email is available",
  "errors": null,
  "data": true
}
```

#### ✅ 200 OK - Email Not Available (Already Used)
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Email is not available",
  "errors": null,
  "data": false
}
```

#### ❌ 400 Bad Request - Invalid Email Format
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Email is not a valid email address"
  ],
  "data": null
}
```

**Specific Error Examples:**
```json
// Empty email
{
  "errors": ["Email can't be empty"]
}

// Email too long
{
  "errors": ["Email cannot exceed 100 characters"]
}

// Invalid format
{
  "errors": ["Email is not a valid email address"]
}
```

---

## 5. Add User - Add User (Admin Only)

### Endpoint
```http
POST /api/user/add-user
```

### Description
Add a new user by Admin or SuperAdmin.

### Authentication
- **Required**: `SuperAdmin` or `Admin`
- **Header**: `Authorization: Bearer {token}`

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `FirstName` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Min Length**: 3 characters<br>- **Max Length**: 15 characters<br>- **Pattern**: Letters only `^[\p{L}]+$` |
| `LastName` | string | ✅ | - **Required**<br>- **Min Length**: 3 characters<br>- **Max Length**: 15 characters<br>- **Pattern**: Letters only `^[\p{L}]+$` |
| `Email` | string | ✅ | - **Required**<br>- **Max Length**: 35 characters<br>- **Format**: Valid Email<br>- **Pattern**: `@.+\..+` |
| `Password` | string | ✅ | - **Required**<br>- **Min Length**: 3 characters |
| `ConfirmPassword` | string | ✅ | - **Required**<br>- **Must Match**: Password |
| `Address` | string | ❌ | - **Optional** |
| `PhoneNumber` | string | ❌ | - **Optional**<br>- **Pattern**: `^\+?[0-9]\d{1,14}$` |
| `UserRole` | enum | ❌ | - **Optional**<br>- **Default**: User (3)<br>- **Values**: SuperAdmin=0, Admin=1, Worker=2, User=3<br>- **Validation**: Must be a valid Enum value |

### Request Example
```json
{
  "firstName": "Omar",
  "lastName": "Hassan",
  "email": "omar.hassan@example.com",
  "password": "SecurePass123",
  "confirmPassword": "SecurePass123",
  "address": "Giza, Egypt",
  "phoneNumber": "+201122334455",
  "userRole": 1
}
```

### Response Codes

#### ✅ 201 Created - User Added Successfully
```json
{
  "statusCode": 201,
  "meta": null,
  "succeeded": true,
  "message": "User added successfully",
  "errors": null,
  "data": {
    "id": 25,
    "email": "omar.hassan@example.com",
    "firstName": "Omar",
    "lastName": "Hassan",
    "pointsBalance": 0,
    "profileImage": null
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
    "FirstName can't be empty",
    "Password does not match",
    "Invalid role"
  ],
  "data": null
}
```

**Example of Invalid UserRole Error:**
```json
{
  "errors": ["Invalid role"]
}
```

#### ❌ 401 Unauthorized - Unauthorized (Missing Token)
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

#### ❌ 403 Forbidden - Forbidden (Not Admin)
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

#### ❌ 409 Conflict - User Already Exists
```json
{
  "statusCode": 409,
  "meta": null,
  "succeeded": false,
  "message": "User already exists",
  "errors": [
    "Email is already registered"
  ],
  "data": null
}
```

---

## 6. Update User - Update User Data

### Endpoint
```http
PATCH /api/user/{id}
```

### Description
Update current user data.

### Authentication
- **Required**: User must be authenticated
- **Header**: `Authorization: Bearer {token}`

### Authorization
- Users can **only update their own data** (when `id` matches their user ID)
- Admin/SuperAdmin can **update any user's data**

### Route Parameters

| Parameter | Type | Required | Validation Rules |
|-----------|------|----------|------------------|
| `id` | int | ✅ | - **Required**<br>- **Type**: Positive integer<br>- **Constraint**: `{id:int}` in route<br>- **Example**: 1, 25, 100 |

### Request Body (multipart/form-data)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `FirstName` | string | ❌ | - **Optional**<br>- If provided, must be valid |
| `LastName` | string | ❌ | - **Optional**<br>- If provided, must be valid |
| `Address` | string | ❌ | - **Optional**<br>- **Min Length**: 4 characters (if provided)<br>- **Max Length**: 20 characters |
| `PhoneNumber` | string | ❌ | - **Optional**<br>- **Pattern**: `^\+?[0-9]\d{1,14}$`<br>- **Format**: Valid international number (2-15 digits)<br>- **Valid Example**: "+201234567890", "01234567890"<br>- **Invalid Example**: "123", "abc123" |
| `ProfileImage` | IFormFile | ❌ | - **Optional**<br>- **Max Size**: 5MB (5242880 bytes)<br>- **Allowed Types**: JPG, JPEG, PNG, GIF, WEBP<br>- **Error Message (Size)**: "Profile image size must not exceed 5MB"<br>- **Error Message (Type)**: "Only image files (jpg, jpeg, png, gif, webp) are allowed" |

**Important Note**: At least **one field** must be sent for update

### Request Example (Form Data)
```bash
PATCH /api/user/15
Content-Type: multipart/form-data

FirstName: Ahmed
LastName: Mohamed
Address: New Cairo, Egypt
PhoneNumber: +201234567890
ProfileImage: [binary file data]
```

### Response Codes

#### ✅ 200 OK - Update Successful
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "User updated successfully",
  "errors": null,
  "data": null
}
```

#### ❌ 400 Bad Request - Invalid Data

**No fields to update:**
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Nothing to change. At least one property must be provided for update."
  ],
  "data": null
}
```

**Address too short:**
```json
{
  "errors": ["Address must be at least 4 characters"]
}
```

**Address too long:**
```json
{
  "errors": ["Address must not exceed 20 characters"]
}
```

**Invalid phone number:**
```json
{
  "errors": ["Phone number is not valid."]
}
```

**Profile image too large:**
```json
{
  "errors": ["Profile image size must not exceed 5MB"]
}
```

**Invalid image type:**
```json
{
  "errors": ["Only image files (jpg, jpeg, png, gif, webp) are allowed"]
}
```

#### ❌ 404 Not Found - User Not Found
```json
{
  "statusCode": 404,
  "meta": null,
  "succeeded": false,
  "message": "User not found",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Not Logged In
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

#### ❌ 403 Forbidden - Attempt to Update Another User
```json
{
  "statusCode": 403,
  "meta": null,
  "succeeded": false,
  "message": "You are not authorized to update this user",
  "errors": null,
  "data": null
}
```



