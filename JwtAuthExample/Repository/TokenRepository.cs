using JwtAuthExample.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthExample.Repository
{
    public class TokenRepository : ITokenRepository
    {

        private readonly JwtAuthConfig _jwtAuthConfig;
        private readonly TokenDbContext _context;
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        public TokenRepository(IOptions<JwtAuthConfig> jwtAuthConfig, TokenDbContext context)
        {
            _jwtAuthConfig = jwtAuthConfig.Value;
            _context = context;
        }

        public async Task<TokenViewModel> GetToken(AuthenticationModel model)
        {

            var user = _context.Users.FirstOrDefault(x => x.Password == model.Password && x.Email == model.Email);

            if (user == null)
                return null;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, model.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtAuthConfig.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtAuthConfig.IssuedAt).ToString(), ClaimValueTypes.Integer64)
            };

            var token = new Models.Token
            {
                AccessToken = GenerateToken(claims),
                RefreshToken = GenerateRefreshToken(),
                ExpiredIn = _jwtAuthConfig.Expiration,
                UserId = user.Id
            };
            _context.Tokens.Add(token);
            var result = _context.SaveChanges();

            return new TokenViewModel
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                ExpiresIn = (int)_jwtAuthConfig.ValidFor.TotalSeconds
            };
        }

        public TokenViewModel RefreshToken(RefreshTokenModel model)
        {

            // retrieve the refresh token from a data store
            var savedRefreshToken = _context
                .Tokens
                .AsNoTracking()
                .FirstOrDefault(x => x.RefreshToken == model.RefreshToken);

            if (savedRefreshToken == null)
                throw new SecurityTokenException("Invalid refresh token");

            var principal = GetPrincipalFromExpiredToken(model.ExpiredToken);
            var username = principal.Identity.Name;

            var newJwtToken = GenerateToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            // Delete Old Refresh Token
            var exTokens = _context
                .Tokens
                .Where(x => x.RefreshToken == model.RefreshToken);

            if (exTokens != null && exTokens.Count() > 0)
            {
                _context.Tokens.RemoveRange(exTokens);
            }

            // Save New Refresh Token
            _context
                .Tokens
                .Add(new Models.Token
                {
                    AccessToken = newJwtToken,
                    RefreshToken = newRefreshToken,
                    ExpiredIn = _jwtAuthConfig.Expiration,
                    UserId = savedRefreshToken.UserId
                });

            var result = _context.SaveChanges();

            return new TokenViewModel
            {
                AccessToken = newJwtToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = (int)_jwtAuthConfig.ValidFor.TotalSeconds
            };
        }

        public void RevokeToken(string refreshToken)
        {
            var tokens = _context.Tokens.Where(x => x.RefreshToken == refreshToken);
            if (tokens != null && tokens.Count() > 0)
            {
                _context.Tokens.RemoveRange(tokens);
                _context.SaveChanges();
            }
        }

        // Generate Token
        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var jwt = new JwtSecurityToken(
                issuer: _jwtAuthConfig.Issuer,
                audience: _jwtAuthConfig.Audience,
                claims: claims,
                notBefore: _jwtAuthConfig.NotBefore,
                expires: _jwtAuthConfig.Expiration,
                signingCredentials: _jwtAuthConfig.SigningCredentials);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        // Get Claim Principle
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidIssuer = _jwtAuthConfig.Issuer,
                ValidateIssuer = true,
                ValidAudience = _jwtAuthConfig.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthConfig.SecretKey)),
                // We don't care about the token's expiration date
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        // Generate Refresh Token
        private string GenerateRefreshToken()
        {
            var bytes = new byte[32];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

    }
}
