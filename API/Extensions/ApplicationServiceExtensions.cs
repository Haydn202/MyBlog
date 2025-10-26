using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.Behaviours;
using API.Data;
using API.Exceptions;
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
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddHttpContextAccessor();
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
            configuration.AddOpenBehavior(typeof(RequestResponseLoggingBehaviour<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<ITokenService, TokenService>(); 
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        //services.AddValidatorsFromAssemblyContaining<RegistrationValidator>();

        return services;
    }
}