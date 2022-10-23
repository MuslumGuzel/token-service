using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using token_service.Models;

namespace token_service.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration configuration;

    public AuthController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpPost("sign-in")]
    public AuthToken? Post([FromBody] User user)
    {
        var now = DateTime.UtcNow;

        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Audience:SecretKey"]));

        var jwt = new JwtSecurityToken(
            issuer: configuration["Audience:Iss"],
            audience: configuration["Audience:Aud"],
            claims: claims,
            notBefore: now,            
            expires: now.Add(TimeSpan.FromMinutes(60)),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
             );

        return new AuthToken
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            Expires = TimeSpan.FromMinutes(2).TotalSeconds
        };
    }

    public class Audience
    {
        public string Secret { get; set; } = null!;
        public string Iss { get; set; } = null!;
        public string Aud { get; set; } = null!;
    }
}
