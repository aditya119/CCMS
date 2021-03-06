﻿using CCMS.Server.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CCMS.Server.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly JwtConfigModel _jwtConfig;

        public CryptoService(JwtConfigModel jwtConfig)
        {
            _jwtConfig = jwtConfig;
        }
        public string SaltAndHashText(string text, string salt)
        {
            using var sha = SHA256.Create();
            var saltedText = text + salt;
            var hash = sha.ComputeHash(Encoding.Unicode.GetBytes(saltedText));
            return Convert.ToBase64String(hash);
        }

        public string GenerateRandomSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public string GenerateJSONWebToken(JwtDetailsModel jwtDetailsModel)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, jwtDetailsModel.Guid),
                new Claim(ClaimTypes.NameIdentifier, jwtDetailsModel.UserId.ToString()),
                new Claim(ClaimTypes.Email, jwtDetailsModel.UserEmail),
                new Claim("platformId", jwtDetailsModel.PlatformId.ToString())
            };
            foreach (var stringRole in jwtDetailsModel.RolesArray)
            {
                claims.Add(new Claim(ClaimTypes.Role, stringRole.Trim()));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwtConfig.ExpiryInDays),
                notBefore: DateTime.Now,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
