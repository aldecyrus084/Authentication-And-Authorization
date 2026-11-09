using Microsoft.IdentityModel.Tokens;
using MyApp.Application.DTOs;
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

        public AuthResponseDTO GenerateToken(Users user)
        {
            var issuer = _jwtSettings.Issuer;
            var audience = _jwtSettings.Audience;
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
            var tokenValidity = _jwtSettings.ExpirationMinute;
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidity);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: tokenExpiryTimeStamp,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            );

            return new AuthResponseDTO
            {
                UserId = user.userId,
                Name = user.Name,
                Email = user.Email,
                Token = token.ToString(),
                TokenValidityMin = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds
            };
        }
    }
}
