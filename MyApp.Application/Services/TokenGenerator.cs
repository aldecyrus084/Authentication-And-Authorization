using Microsoft.IdentityModel.Tokens;
using MyApp.Application.Interfaces.Services;
using MyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApp.Application.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public TokenGenerator(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateToken(Users user)
        {
            // 1. Create the secret key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)
            );

            // 2. Create signing credentials
            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            // 3. Prepare user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),

                new Claim(ClaimTypes.Name, user.Name),

                new Claim(ClaimTypes.Email, user.Email),

            };

            // 4. Create JWT token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _jwtSettings.ExpirationMinute
                ),
                signingCredentials: credentials
            );

            // 5. Convert token into a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
