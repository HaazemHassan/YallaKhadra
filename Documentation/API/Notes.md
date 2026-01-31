# General Notes

## Response Structure
All responses follow this structure:

```typescript
{
  statusCode: number,      // HTTP Status Code
  meta: object | null,     // Additional metadata
  succeeded: boolean,      // Operation success status
  message: string,         // Descriptive message
  errors: string[] | null, // List of errors (if any)
  data: T | null          // Returned data
}
```

## Common Error Messages

### Validation Errors
- `"{PropertyName} can't be empty"` - Field is empty
- `"{PropertyName} can't be null"` - Field is null
- `"Invalid email address format."` - Incorrect email format
- `"Password does not match"` - Passwords do not match
- `"Phone number is not valid."` - Invalid phone number
- `"The length of '{PropertyName}' must be at least {MinLength} characters."` - Length is less than required
- `"The length of '{PropertyName}' must be {MaxLength} characters or fewer."` - Length exceeds allowed limit

### Business Logic Errors
- `"User already exists"` - User already exists
- `"User not found"` - User not found
- `"Email is already registered"` - Email is already registered
- `"Phone number is already in use"` - Phone number is already in use

### Authorization Errors
- `"Unauthorized"` - Unauthorized (Missing token)
- `"Access denied. Insufficient permissions."` - Insufficient permissions
- `"Access denied. This endpoint is for anonymous users only."` - For anonymous users only

## Security Headers
All responses contain security headers:
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `X-XSS-Protection: 1; mode=block`
- `Strict-Transport-Security: max-age=31536000; includeSubDomains`

## Content Type
- **Request**: 
  - `/register`: `multipart/form-data`
  - Other endpoints: `application/json`
- **Response**: `application/json`
