using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthExample
{
    public static class JwtAuthExtensions
    {

        static string _secretKey;
        static SymmetricSecurityKey _securityKey;
        static SigningCredentials _signingKey;

        public static void AddJwt(this IServiceCollection services, IConfiguration config)
        {

            var jwtAuthConfig = config.GetSection(nameof(JwtAuthConfig));

            _secretKey = jwtAuthConfig[nameof(JwtAuthConfig.SecretKey)];
            _securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
            _signingKey = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

            services.Configure<JwtAuthConfig>(option =>
            {
                option.Issuer = jwtAuthConfig[nameof(JwtAuthConfig.Issuer)];
                option.Audience = jwtAuthConfig[nameof(JwtAuthConfig.Audience)];
                option.SecretKey = _secretKey;
                option.SigningCredentials = _signingKey;
            });
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = CreateTokenParameters(config);
                option.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "Yes");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        }

        static TokenValidationParameters CreateTokenParameters(IConfiguration config)
        {
            var _jwtAuthConfig = config.GetSection(nameof(JwtAuthConfig));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtAuthConfig[nameof(JwtAuthConfig.Issuer)],

                ValidateAudience = true,
                ValidAudience = _jwtAuthConfig[nameof(JwtAuthConfig.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _securityKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };
            return tokenValidationParameters;
        }

    }
}
