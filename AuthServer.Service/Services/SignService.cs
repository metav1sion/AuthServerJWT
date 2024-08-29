using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Service.Services;

internal static class SignService
{
    public static SecurityKey GetSymmetricSecurityKey(string securtiyKey)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securtiyKey));
    }
}