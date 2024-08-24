using AuthServer.Core.Dtos;
using AuthServer.SharedLibrary.Dtos;

namespace AuthServer.Core.Services;

public interface IAuthenticationService
{
    Task<ResponseDto<TokenDTO>> CreateTokenAsync(LoginDTO loginDto);
    Task<ResponseDto<TokenDTO>> CreateTokenByRefreshTokenAsync(string refreshToken);
    Task<ResponseDto<NoDataDTO>> RevokeRefreshTokenAsync(string refreshToken); //Refresh token kaldırmak için kullanılır.
    Task<ResponseDto<ClientTokenDTO>> CreateTokenByClient(ClientLoginDTO clientLoginDto);
}