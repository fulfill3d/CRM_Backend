using CRM.Common.Database;
using CRM.Common.Services;
using CRM.Common.Services.Interfaces;
using CRM.Common.Services.Options;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Common.DI
{
    public static class DepInj
    {
        public static void AddDatabaseContext<TContext>(
            this IServiceCollection services, DatabaseOption dbConnectionsOptions)
            where TContext : DbContext
        {
            
            services.AddDbContext<TContext>(
                options =>
                {
                    if (!options.IsConfigured)
                    {
                        options.UseSqlServer(dbConnectionsOptions.ConnectionString, opt =>
                        {
                            opt.EnableRetryOnFailure(
                                maxRetryCount: 5, // Number of retry attempts
                                maxRetryDelay: TimeSpan.FromSeconds(30), // Maximum delay between retries
                                errorNumbersToAdd: null); // Add additional SQL error numbers to retry on
                            opt.UseNetTopologySuite();
                        });
                    }
                });
        }
        
        public static void ConfigureServiceOptions<TOptions>(
            this IServiceCollection services,
            Action<IServiceProvider, TOptions> configure)
            where TOptions : class
        {
            services
                .AddOptions<TOptions>()
                .Configure<IServiceProvider>((options, resolver) => configure(resolver, options));
        }
        
        public static void AddB2CJwtTokenValidator(
            this IServiceCollection services,
            Action<IServiceProvider, TokenValidationOptions> opt)
        {
            services.ConfigureServiceOptions(opt);
            services.AddSingleton<IJwtValidatorService, JwtValidatorService>();
        }
        
        public static void AddHttpRequestBodyMapper(this IServiceCollection services)
        {
            services.AddTransient(typeof(IHttpRequestBodyMapper<>), typeof(HttpRequestBodyMapper<>));
        }

        public static void AddFluentValidator<T>(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(T).Assembly);
        }
    }
}