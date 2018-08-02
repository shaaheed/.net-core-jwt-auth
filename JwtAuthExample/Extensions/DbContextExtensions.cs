using JwtAuthExample.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace JwtAuthExample
{
    public static class DbContextExtensions
    {

        public static void AddDbContext(this IServiceCollection services, IConfiguration config)
        {

            var connectionString = config.GetSection("DatabaseConfiguration:ConnectionString").Value;

            services.AddDbContext<TokenDbContext>(option =>
            {
                option.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>

                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                    sqlOptions.MigrationsAssembly(typeof(TokenDbContext).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            services.AddTransient<ITokenRepository, TokenRepository>();

        }

    }
}
