using API.Behaviours;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load CORS settings from configuration
var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>();
if (corsSettings == null || corsSettings.AllowedOrigins.Length == 0)
{
    throw new Exception("CorsSettings configuration is missing or empty");
}

// ✅ Register CORS services (YOU WERE MISSING THIS)
builder.Services.AddCors();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ CORS MUST be before auth & controllers
app.UseCors(options => options
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .WithOrigins(corsSettings.AllowedOrigins)
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
