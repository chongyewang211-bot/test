using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using test.Configuration;
using test.Services;
using test.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

builder.Services.AddSingleton<MongoDbService>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProblemService, ProblemService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var issuer = jwtSettings["Issuer"] ?? "TestApp";
var audience = jwtSettings["Audience"] ?? "TestAppUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    
    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // Serve the Swagger JSON endpoint
    app.UseSwaggerUI();     // Serve the Swagger web UI
} 

app.UseSwagger();       // Serve the Swagger JSON endpoint
app.UseSwaggerUI();     // Serve the Swagger web UI

app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Seed data on startup (with error handling)
try
{
    using (var scope = app.Services.CreateScope())
    {
        var problemService = scope.ServiceProvider.GetRequiredService<IProblemService>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        
        await problemService.SeedDataAsync();
        await SeedUsersAsync(userService);
        
        Console.WriteLine("✅ Data seeded successfully!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️  Warning: Could not seed data to MongoDB: {ex.Message}");
    Console.WriteLine("The application will continue to run, but some features may not work properly.");
    Console.WriteLine("Please check your MongoDB connection string and credentials.");
}

app.MapControllers();
app.Run();

// Helper method to seed users
static async Task SeedUsersAsync(IUserService userService)
{
    try
    {
        // Check if test user already exists
        var existingUser = await userService.GetUserByUsernameAsync("test");
        if (existingUser == null)
        {
            // Create test user with username: "test", password: "test"
            await userService.CreateUserAsync("test", "test@example.com", "test");
            Console.WriteLine("✅ Test user created: username='test', password='test'");
        }
        else
        {
            Console.WriteLine("ℹ️  Test user already exists: username='test'");
        }

        // Only create the test user - no additional demo credentials
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Warning: Could not create users: {ex.Message}");
    }
}
