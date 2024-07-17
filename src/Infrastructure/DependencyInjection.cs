using System.Diagnostics;
using System.Security.Claims;
using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Infrastructure.Data;
using BowlingApp.Infrastructure.Data.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>(
            (sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                options
                    .UseNpgsql(
                        connectionString,
                        o =>
                            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                                .EnableRetryOnFailure(maxRetryCount: 5)
                    )
                    .LogTo(
                        s => Debug.WriteLine(s),
                        new[] { DbLoggerCategory.Database.Command.Name },
                        LogLevel.Information
                    );
            }
        );

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = configuration.GetValue<string>("Auth:Authority");
            options.Audience = configuration.GetValue<string>("Auth:Audience");
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                NameClaimType = ClaimTypes.NameIdentifier
            };
        });

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
