# Authentication API Documentation

---

> **Note**: For general notes including response structure, error messages see [Notes.md](Notes.md)

---

## Overview
This controller handles user authentication operations including login, token refresh, logout, and password management.

---

## 1. Login - Authenticate User

### Endpoint
```http
POST /api/authentication/login
```

### Description
Authenticate a user with email and password. Returns JWT access token and user information.

### Authentication
- **Anonymous Only** - User must not be authenticated (for new login sessions)

### Rate Limiting
- **Limit**: 5 attempts per minute per IP address
- **Policy**: loginLimiter

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `Email` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Valid Example**: "ahmed@example.com" |
| `Password` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Valid Example**: "Pass123" |

### Request Example
```json
{
  "email": "ahmed@example.com",
  "password": "Pass123"
}
```

### Response Codes

#### ✅ 200 OK - Login Successful
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Login successful",
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
      "email": "ahmed@example.com",
      "firstName": "Ahmed",
      "lastName": "Ali",
      "pointsBalance": 150,
      "profileImage": {
        "id": 42,
        "url": "https://storage.example.com/profiles/15/profile.jpg"
      }
    }
  }
}
```

**For Web Clients:**
- Refresh token is stored in HTTP-only cookie named `refreshToken`
- The `refreshToken` field in the response will be `null`
- Cookie settings: HttpOnly, Secure, SameSite=None, Path=/api/authentication

**For Mobile/Non-Web Clients:**
- Refresh token is included in the response body
- Client must store and send it manually for refresh operations

#### ❌ 400 Bad Request - Invalid Data
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "Email can't be empty",
    "Password can't be empty"
  ],
  "data": null
}
```

#### ❌ 401 Unauthorized - Invalid Credentials
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Invalid email or password",
  "errors": null,
  "data": null
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

#### ❌ 429 Too Many Requests - Rate Limit Exceeded
```json
{
  "statusCode": 429,
  "meta": null,
  "succeeded": false,
  "message": "Too many login attempts. Please try again later.",
  "errors": null,
  "data": null
}
```

---

## 2. Refresh Token - Refresh Expired Access Token

### Endpoint
```http
POST /api/authentication/refresh-token
```

### Description
Refresh an expired access token using the refresh token. A new refresh token is also generated and the old one is revoked.

### Authentication
- No requirements

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `AccessToken` | string | ✅ | - **Required**<br>- Current (possibly expired) access token<br>- Must be valid in format |
| `RefreshToken` | string | ❌ | - **Optional for web clients** (read from HTTP-only cookie)<br>- **Required for non-web clients**<br>- Valid refresh token |

### Request Example

**For Web Clients:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```
*Note: Refresh token is automatically read from the `refreshToken` cookie*

**For Non-Web Clients:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0"
}
```

### Response Codes

#### ✅ 200 OK - Token Refreshed Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Token refreshed successfully",
  "errors": null,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": {
      "token": "new_refresh_token_here",
      "userId": 15,
      "expirationDate": "2026-03-15T10:30:00Z",
      "createdAt": "2026-02-01T10:30:00Z"
    },
    "user": {
      "id": 15,
      "email": "ahmed@example.com",
      "firstName": "Ahmed",
      "lastName": "Ali",
      "pointsBalance": 150,
      "profileImage": {
        "id": 42,
        "url": "https://storage.example.com/profiles/15/profile.jpg"
      }
    }
  }
}
```

**For Web Clients:**
- New refresh token is stored in HTTP-only cookie
- Old refresh token is revoked
- The `refreshToken` field in the response will be `null`

#### ❌ 400 Bad Request - Invalid Access Token Format
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Invalid access token format",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Invalid Refresh Token
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Invalid Refresh token.",
  "errors": null,
  "data": null
}
```

**Or when refresh token is expired:**
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Refresh token has expired",
  "errors": null,
  "data": null
}
```

---

## 3. Logout - Logout Current User

### Endpoint
```http
POST /api/authentication/logout
```

### Description
Logout the current user and revoke the refresh token. For web clients, the refresh token cookie is also deleted.

### Authentication
- **Required**: User must be authenticated
- **Header**: `Authorization: Bearer {token}`

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `RefreshToken` | string | ❌ | - **Optional for web clients** (read from HTTP-only cookie)<br>- **Required for non-web clients**<br>- Valid refresh token to revoke |

### Request Example

**For Web Clients:**
```json
{}
```
*Note: Refresh token is automatically read from the `refreshToken` cookie*

**For Non-Web Clients:**
```json
{
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0"
}
```

### Response Codes

#### ✅ 200 OK - Logout Successful
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Logged out successfully",
  "errors": null,
  "data": true
}
```

**For Web Clients:**
- The `refreshToken` cookie is deleted

#### ❌ 400 Bad Request - Logout Failed
```json
{
  "statusCode": 400,
  "meta": null,
  "succeeded": false,
  "message": "Logout failed",
  "errors": null,
  "data": null
}
```

#### ❌ 401 Unauthorized - Missing Refresh Token
```json
{
  "statusCode": 401,
  "meta": null,
  "succeeded": false,
  "message": "Refresh token is required",
  "errors": null,
  "data": null
}
```

**Or not authenticated:**
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

## 4. Change Password - Change Current User Password

### Endpoint
```http
PATCH /api/authentication/change-password
```

### Description
Change the password for the currently authenticated user.

### Authentication
- **Required**: User must be authenticated
- **Header**: `Authorization: Bearer {token}`

### Request Body (application/json)

| Property | Type | Required | Validation Rules |
|----------|------|----------|------------------|
| `CurrentPassword` | string | ✅ | - **Required** (cannot be empty or null) |
| `NewPassword` | string | ✅ | - **Required** (cannot be empty or null)<br>- **Min Length**: 3 characters |
| `ConfirmNewPassword` | string | ✅ | - **Required**<br>- **Must Match**: NewPassword<br>- **Error Message**: "Password does not match" |

### Request Example
```json
{
  "currentPassword": "OldPass123",
  "newPassword": "NewSecurePass456",
  "confirmNewPassword": "NewSecurePass456"
}
```

### Response Codes

#### ✅ 200 OK - Password Changed Successfully
```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Password changed successfully",
  "errors": null,
  "data": null
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
    "CurrentPassword can't be empty",
    "NewPassword must be at least of length 3",
    "Password does not match"
  ],
  "data": null
}
```

**Specific Validation Error Examples:**
```json
// Passwords don't match
{
  "errors": ["Password does not match"]
}

// New password too short
{
  "errors": ["NewPassword must be at least of length 3"]
}

// Current password is wrong
{
  "statusCode": 400,
  "succeeded": false,
  "message": "Current password is incorrect",
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