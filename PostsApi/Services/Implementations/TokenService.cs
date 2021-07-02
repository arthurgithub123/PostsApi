using Microsoft.IdentityModel.Tokens;
using PostsApi.Models.Entities.Identity;
using PostsApi.Models.JsonWebToken;
using PostsApi.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PostsApi.Services.Implementations
{
    public class TokenService : ITokenService
    {
        public UserToken BuildToken(ApplicationUser applicationUser, string role, string jwtSecretKey)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id.ToString()),
                new Claim("role", role)
            };

            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));

            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            DateTime expirationTime = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                claims: claims,
                expires: expirationTime,
                signingCredentials: signingCredentials
            );

            UserInfo userInfo = new UserInfo
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                Name = applicationUser.Name,
                Role = role
            };

            UserToken userToken = new UserToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                User = userInfo
            };

            return userToken;
        }
    }
}
