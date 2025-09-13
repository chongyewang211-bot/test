# Unit Tests for Test Project

This test project contains comprehensive unit tests for the server-side components of the application.

## Test Structure

### Services Tests
- **UserServiceTests.cs**: Tests for user authentication, validation, and CRUD operations
- **ProblemServiceTests.cs**: Tests for problem management, auto-incrementing, and category operations

### Controllers Tests
- **AuthControllerTests.cs**: Tests for login, authentication, and user management endpoints
- **ProblemsControllerTests.cs**: Tests for problem CRUD operations, filtering, and category management

### Models Tests
- **ModelTests.cs**: Tests for model validation, property assignments, and serialization

### Integration Tests
- **ApiIntegrationTests.cs**: End-to-end tests for API endpoints with authentication

## Test Features

### Unit Testing
- ✅ **Mocking**: Uses Moq for dependency injection and external service mocking
- ✅ **Assertions**: Uses FluentAssertions for readable and expressive test assertions
- ✅ **Coverage**: Tests all major functionality including edge cases and error scenarios

### Integration Testing
- ✅ **API Testing**: Uses Microsoft.AspNetCore.Mvc.Testing for full HTTP pipeline testing
- ✅ **Authentication**: Tests JWT token generation and authorization
- ✅ **Database**: Tests auto-incrementing problem numbers and data persistence

## Key Test Scenarios

### Authentication Tests
- Valid login with correct credentials
- Invalid login with wrong username/password
- Inactive user login attempts
- JWT token generation and validation

### Problem Management Tests
- Auto-incrementing problem numbers (1, 2, 3, 4, 5...)
- Problem creation with automatic numbering
- Category filtering and search
- CRUD operations for problems and categories

### Data Validation Tests
- Model property validation
- Required field checking
- Data type validation
- Serialization/deserialization

## Running Tests

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "ClassName=UserServiceTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test Dependencies

- **xUnit**: Testing framework
- **Moq**: Mocking framework
- **FluentAssertions**: Assertion library
- **Microsoft.AspNetCore.Mvc.Testing**: Integration testing
- **coverlet.collector**: Code coverage collection

## Test Configuration

Tests use a separate configuration file (`appsettings.Test.json`) with:
- Test database connection string
- Test JWT settings
- Logging configuration for tests

## Coverage Areas

### ✅ Covered
- User authentication and authorization
- Problem CRUD operations
- Category management
- Auto-incrementing problem numbers
- API endpoint validation
- Error handling and edge cases
- Model validation and serialization

### 🔄 Integration Tests
- End-to-end API testing
- Authentication flow testing
- Database operations testing
- Auto-increment functionality verification

## Test Data

Tests use mock data and isolated test scenarios to ensure:
- No interference between tests
- Predictable test outcomes
- Fast test execution
- Reliable CI/CD pipeline integration

## Future Enhancements

- Performance testing
- Load testing for API endpoints
- Security testing for authentication
- Database migration testing
- Frontend integration testing
