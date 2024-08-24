using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;

namespace AuthServer.Core.Services;

public interface ITokenService
{
    TokenDTO CreateToken(UserApp userApp);
    ClientTokenDTO createTokenByClient(Client client);
}