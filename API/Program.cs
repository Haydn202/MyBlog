using API.Behaviours;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CORS: allow env var ALLOWED_ORIGINS (comma-separated) to override config for runtime (e.g. Container App)
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .ToArray();
if (allowedOrigins is null || allowedOrigins.Length == 0)
{
    var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>();
    if (corsSettings == null || corsSettings.AllowedOrigins.Length == 0)
        throw new Exception("CorsSettings.AllowedOrigins or ALLOWED_ORIGINS env var is required");
    allowedOrigins = corsSettings.AllowedOrigins;
}

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// No UseHttpsRedirection: TLS at edge; redirect causes 301 loop behind proxy

// CORS before auth so preflight OPTIONS gets CORS headers without requiring auth
app.UseCors(policy => policy
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .WithOrigins(allowedOrigins)
    .WithExposedHeaders("X-Refresh-Token"));

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

// Root health check (used by Container Apps liveness probe and to verify API is up)
app.MapGet("/", () => Results.Ok("OK"));

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();

    var adminSettings = builder.Configuration.GetSection("AdminSettings").Get<AdminSettings>();
    if (adminSettings == null)
    {
        throw new Exception("AdminSettings configuration is missing");
    }

    var userManager = services.GetRequiredService<UserManager<User>>();
    await SeedData.InitialiseAdmin(userManager, adminSettings);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

app.Run();
