using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
