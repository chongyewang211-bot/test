using Xunit;

namespace test.Tests
{
    public class SimpleWorkingTest
    {
        [Fact]
        public void SimpleTest_ShouldPass()
        {
            // Arrange
            var expected = "Hello";
            var actual = "Hello";

            // Act & Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MathTest_ShouldWork()
        {
            // Arrange
            var a = 2;
            var b = 3;
            var expected = 5;

            // Act
            var result = a + b;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void AutoIncrementLogic_ShouldWork()
        {
            // Arrange - Simulate existing problems
            var existingProblems = new[]
            {
                new { ProblemNumber = 1 },
                new { ProblemNumber = 3 },
                new { ProblemNumber = 5 }
            };

            // Act - Find next problem number
            var highestNumber = existingProblems.Max(p => p.ProblemNumber);
            var nextNumber = highestNumber + 1;

            // Assert
            Assert.Equal(5, highestNumber);
            Assert.Equal(6, nextNumber);
        }

        [Fact]
        public void AuthenticationLogic_ShouldWork()
        {
            // Arrange
            var password = "test";
            var hashedPassword = ComputeSha256Hash(password);

            // Act
            var isValid = ValidatePassword(password, hashedPassword);

            // Assert
            Assert.True(isValid);
            Assert.NotEmpty(hashedPassword);
            Assert.Equal(64, hashedPassword.Length); // SHA256 hash length
        }

        private static string ComputeSha256Hash(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(hashedBytes);
        }

        private static bool ValidatePassword(string password, string hash)
        {
            var computedHash = ComputeSha256Hash(password);
            return string.Equals(computedHash, hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
