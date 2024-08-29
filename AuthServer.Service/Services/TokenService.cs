using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.Services;
using AuthServer.SharedLibrary.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Service.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<UserApp> _userManager;
    private readonly CustomTokenOption _customTokenOption;


    public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> customTokenOption)
    {
        _userManager = userManager;
        _customTokenOption = customTokenOption.Value;
    }

    private IEnumerable<Claim> GetClaims(UserApp userApp, List<string> audiences)
    {
        var userList = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier,userApp.Id),
            new Claim(JwtRegisteredClaimNames.Email,userApp.Email), //hangisini kullanmak istersen
            new Claim(ClaimTypes.Name,userApp.UserName),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), //tokena identity verir

        };
        userList.AddRange(audiences.Select(x=>new Claim(JwtRegisteredClaimNames.Aud,x)));
        
        return userList;
    }

    private IEnumerable<Claim> GetClaimsByClient(Client client)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString()),
        };
        claims.AddRange(client.Audieneces.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

        return claims;
    }


    private string CreateRefreshToken()
    {
        //return Guid.NewGuid().ToString(); böyle de dönülebilir
        var numberByte = new Byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(numberByte);

        return Convert.ToBase64String(numberByte);
    }

    public TokenDTO CreateToken(UserApp userApp)
    {
        var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);
        var refreshTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.RefreshTokenExpiration);
        var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);

        SigningCredentials signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256Signature);

        JwtSecurityToken jwtSecurityTokentoken = new JwtSecurityToken(

            issuer: _customTokenOption.Issuer,
            expires: accessTokenExpiration,
            notBefore: DateTime.Now,
            claims: GetClaims(userApp, _customTokenOption.Audience),
            signingCredentials: signingCredentials

        );

        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(jwtSecurityTokentoken);
        var tokenDto = new TokenDTO
        {
            AccessToken = token,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshToken = CreateRefreshToken(),
            RefreshTokenExpiration = refreshTokenExpiration.ToString(),
        };
        return tokenDto;
    }

    public ClientTokenDTO createTokenByClient(Client client)
    {
        var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);
        var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);

        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        JwtSecurityToken jwtSecurityTokentoken = new JwtSecurityToken(

            issuer: _customTokenOption.Issuer,
            expires: accessTokenExpiration,
            notBefore: DateTime.Now,
            claims: GetClaimsByClient(client),
            signingCredentials: signingCredentials

        );

        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(jwtSecurityTokentoken);
        var tokenDto = new ClientTokenDTO
        {
            AccessToken = token,
            AccessTokenExpiration = accessTokenExpiration
        };
        return tokenDto;
    }
}