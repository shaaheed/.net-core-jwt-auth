using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Msi.JwtAuth
{
    public static class JwtAuthExtensions
    {
        public static void AddJwtAuth(this IServiceCollection services, IConfiguration config)
        {
            var configSection = config.GetSection(nameof(JwtAuthConfig));
            services.Configure<JwtAuthConfig>(configSection);
            services.AddTransient<IJwtAuth, JwtAuth>();
        }

        public static void AddDefaultJwtAuthTokenParameters(this IServiceCollection services, IConfiguration config)
        {
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };
        }

    }
}
