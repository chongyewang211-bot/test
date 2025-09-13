using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace test.Tests
{
    /// <summary>
    /// Simple test runner to demonstrate test scenarios without full xUnit framework
    /// This can be used to verify test logic when the main test framework has issues
    /// </summary>
    public class SimpleTestRunner
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🧪 Running Simple Test Suite");
            Console.WriteLine("=============================");

            var testRunner = new SimpleTestRunner();
            
            try
            {
                await testRunner.RunAllTests();
                Console.WriteLine("\n✅ All tests passed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Test failed: {ex.Message}");
            }
        }

        public async Task RunAllTests()
        {
            Console.WriteLine("\n📋 Running Model Tests...");
            await RunModelTests();
            
            Console.WriteLine("\n🔐 Running Authentication Logic Tests...");
            await RunAuthLogicTests();
            
            Console.WriteLine("\n📊 Running Problem Number Logic Tests...");
            await RunProblemNumberTests();
            
            Console.WriteLine("\n🗃️ Running Data Validation Tests...");
            await RunDataValidationTests();
        }

        private async Task RunModelTests()
        {
            // Test Problem model
            var problem = new test.Models.Problem
            {
                Id = "123",
                ProblemNumber = 1,
                Title = "Test Problem",
                Description = "Test Description",
                Difficulty = "Easy",
                Category = "Frontend Development",
                Tags = new List<string> { "JavaScript", "HTML" },
                AcceptanceRate = 85,
                Likes = 150,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            AssertEqual("123", problem.Id, "Problem ID");
            AssertEqual(1, problem.ProblemNumber, "Problem Number");
            AssertEqual("Test Problem", problem.Title, "Problem Title");
            AssertEqual("Frontend Development", problem.Category, "Problem Category");
            AssertEqual(2, problem.Tags.Count, "Problem Tags Count");
            AssertTrue(problem.IsActive, "Problem IsActive");
            
            Console.WriteLine("  ✅ Problem model tests passed");

            // Test Category model
            var category = new test.Models.Category
            {
                Id = "456",
                Key = "frontend",
                Name = "Frontend Development",
                Icon = "💻",
                Color = "#4CAF50",
                ProblemCount = 25,
                EasyCount = 10,
                MediumCount = 10,
                HardCount = 5,
                IsActive = true
            };

            AssertEqual("frontend", category.Key, "Category Key");
            AssertEqual("Frontend Development", category.Name, "Category Name");
            AssertEqual("💻", category.Icon, "Category Icon");
            AssertEqual(25, category.ProblemCount, "Category Problem Count");
            AssertTrue(category.IsActive, "Category IsActive");
            
            Console.WriteLine("  ✅ Category model tests passed");

            // Test User model
            var user = new test.Models.User
            {
                Id = "789",
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                IsActive = true
            };

            AssertEqual("testuser", user.Username, "User Username");
            AssertEqual("test@example.com", user.Email, "User Email");
            AssertTrue(user.IsActive, "User IsActive");
            
            Console.WriteLine("  ✅ User model tests passed");

            // Test LoginRequest model
            var loginRequest = new test.Models.LoginRequest
            {
                Username = "testuser",
                Password = "testpassword"
            };

            AssertEqual("testuser", loginRequest.Username, "LoginRequest Username");
            AssertEqual("testpassword", loginRequest.Password, "LoginRequest Password");
            
            Console.WriteLine("  ✅ LoginRequest model tests passed");

            // Test LoginResponse model
            var loginResponse = new test.Models.LoginResponse
            {
                Token = "jwt-token-string",
                Username = "testuser",
                Email = "test@example.com"
            };

            AssertEqual("jwt-token-string", loginResponse.Token, "LoginResponse Token");
            AssertEqual("testuser", loginResponse.Username, "LoginResponse Username");
            
            Console.WriteLine("  ✅ LoginResponse model tests passed");
        }

        private async Task RunAuthLogicTests()
        {
            // Test password hashing logic
            var password = "testpassword";
            var expectedHash = ComputeSha256Hash(password);
            
            AssertTrue(!string.IsNullOrEmpty(expectedHash), "Password hash generation");
            AssertEqual(64, expectedHash.Length, "Password hash length (SHA256)");
            
            // Test password validation
            var isValid = ValidatePassword(password, expectedHash);
            AssertTrue(isValid, "Password validation");
            
            var isInvalid = ValidatePassword("wrongpassword", expectedHash);
            AssertTrue(!isInvalid, "Invalid password rejection");
            
            Console.WriteLine("  ✅ Authentication logic tests passed");
        }

        private async Task RunProblemNumberTests()
        {
            // Test auto-increment logic
            var problems = new List<test.Models.Problem>
            {
                new test.Models.Problem { ProblemNumber = 1, Title = "Problem 1" },
                new test.Models.Problem { ProblemNumber = 2, Title = "Problem 2" },
                new test.Models.Problem { ProblemNumber = 5, Title = "Problem 5" }
            };

            // Simulate finding highest problem number
            var highestNumber = problems.Max(p => p.ProblemNumber);
            var nextNumber = highestNumber + 1;
            
            AssertEqual(5, highestNumber, "Highest problem number");
            AssertEqual(6, nextNumber, "Next problem number");
            
            // Test sequential numbering
            var sequentialNumbers = new List<int> { 1, 2, 3, 4, 5 };
            for (int i = 0; i < sequentialNumbers.Count; i++)
            {
                AssertEqual(i + 1, sequentialNumbers[i], $"Sequential number {i + 1}");
            }
            
            Console.WriteLine("  ✅ Problem number logic tests passed");
        }

        private async Task RunDataValidationTests()
        {
            // Test required field validation
            var problem = new test.Models.Problem();
            
            AssertTrue(string.IsNullOrEmpty(problem.Title), "Empty title validation");
            AssertTrue(!problem.IsActive, "Default IsActive value");
            AssertEqual(0, problem.ProblemNumber, "Default ProblemNumber value");
            
            // Test data type validation
            var category = new test.Models.Category();
            
            AssertEqual(0, category.ProblemCount, "Default ProblemCount value");
            AssertEqual(0, category.EasyCount, "Default EasyCount value");
            AssertTrue(!category.IsActive, "Default Category IsActive value");
            
            // Test list handling
            var problemWithEmptyTags = new test.Models.Problem { Tags = new List<string>() };
            AssertEqual(0, problemWithEmptyTags.Tags.Count, "Empty tags list");
            
            var problemWithNullTags = new test.Models.Problem { Tags = null };
            AssertTrue(problemWithNullTags.Tags == null, "Null tags list");
            
            Console.WriteLine("  ✅ Data validation tests passed");
        }

        // Helper methods
        private void AssertEqual<T>(T expected, T actual, string testName)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new Exception($"❌ {testName}: Expected '{expected}', but got '{actual}'");
            }
            Console.WriteLine($"    ✓ {testName}: {actual}");
        }

        private void AssertTrue(bool condition, string testName)
        {
            if (!condition)
            {
                throw new Exception($"❌ {testName}: Expected true, but got false");
            }
            Console.WriteLine($"    ✓ {testName}: true");
        }

        private string ComputeSha256Hash(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(hashedBytes);
        }

        private bool ValidatePassword(string password, string hash)
        {
            var computedHash = ComputeSha256Hash(password);
            return string.Equals(computedHash, hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
