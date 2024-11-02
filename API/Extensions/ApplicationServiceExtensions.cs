using API.Data;
using API.Features.Accounts.Commands;
using API.Interfaces;
using API.Profiles.Resolvers;
using API.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<ITokenService, TokenService>(); 
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddValidatorsFromAssemblyContaining<RegistrationValidator>();

        return services;
    }
}