using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using test.Configuration;

namespace test.Tests.TestHelpers
{
    public static class TestConfiguration
    {
        public static IConfiguration GetTestConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static MongoDbSettings GetTestMongoDbSettings()
        {
            return new MongoDbSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "test_database"
            };
        }

        public static void ConfigureTestServices(IServiceCollection services)
        {
            // Add test-specific services here if needed
            services.AddLogging();
        }
    }

    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real MongoDB service
                var mongoDbServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(MongoDbService));
                if (mongoDbServiceDescriptor != null)
                {
                    services.Remove(mongoDbServiceDescriptor);
                }

                // Add test configuration
                services.Configure<MongoDbSettings>(TestConfiguration.GetTestMongoDbSettings());
                
                // Configure test services
                TestConfiguration.ConfigureTestServices(services);
            });

            builder.UseEnvironment("Testing");
        }
    }
}
