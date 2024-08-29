using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.SharedLibrary.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthServer.Service.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly List<Client> _clients;
    private readonly ITokenService _tokenService;
    private readonly UserManager<UserApp> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

    
    public AuthenticationService(IOptions<List<Client>> options, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
    {
        _clients = options.Value;
        _tokenService = tokenService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _userRefreshTokenService = userRefreshTokenService;
    }

    public async Task<ResponseDto<TokenDTO>> CreateTokenAsync(LoginDTO loginDto)
    {
        if (loginDto == null)
        {
            throw new ArgumentNullException(nameof(loginDto));
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            return ResponseDto<TokenDTO>.Failure("E-Mail or Password is wrong",400,true);
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return ResponseDto<TokenDTO>.Failure("E-Mail or Password is wrong", 400, true);
        }

        var token = _tokenService.CreateToken(user);
        var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

        if (userRefreshToken == null)
        {
            await _userRefreshTokenService.AddAsync(new UserRefreshToken
            {
                UserId = user.Id,
                Code = token.RefreshToken,
                Expiration = token.RefreshTokenExpiration
            });
        }
        else
        {
            userRefreshToken.Code = token.RefreshToken;
            userRefreshToken.Expiration = token.RefreshTokenExpiration;
        }

        await _unitOfWork.CommitAsync();

        return ResponseDto<TokenDTO>.Success(token,200);


    }

    public async Task<ResponseDto<TokenDTO>> CreateTokenByRefreshTokenAsync(string refreshToken)
    {
        var userRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

        // Null kontrolü
        if (userRefreshToken == null)
        {
            return ResponseDto<TokenDTO>.Failure("Invalid refresh token!", 404, true);
        }

        // Tarih kontrolü
        if (userRefreshToken.Expiration <= DateTime.Now)
        {
            return ResponseDto<TokenDTO>.Failure("Refresh token has expired!", 404, true);
        }

        var user = await _userManager.FindByIdAsync(userRefreshToken.UserId);
        if (user == null)
        {
            return ResponseDto<TokenDTO>.Failure("User not found!", 404, true);
        }

        var token = _tokenService.CreateToken(user);

        // Refresh token güncellemesi
        userRefreshToken.Code = token.RefreshToken;
        userRefreshToken.Expiration = token.RefreshTokenExpiration;

        await _unitOfWork.CommitAsync();

        return ResponseDto<TokenDTO>.Success(token, 200);
    }


    public Task<ResponseDto<NoDataDTO>> RevokeRefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public ResponseDto<ClientTokenDTO> CreateTokenByClient(ClientLoginDTO clientLoginDto)
    {
        if (clientLoginDto == null)
        {
            throw new ArgumentNullException(nameof(clientLoginDto));
        }

        var client = _clients.SingleOrDefault(x => x.ClientId == clientLoginDto.ClientId && x.ClientSecret == clientLoginDto.ClientSecret);

        if (client == null)
        {
            return ResponseDto<ClientTokenDTO>.Failure("ClientSecret is wrong", 400, true);
        }

        if (client.ClientSecret != clientLoginDto.ClientSecret)
        {
            return ResponseDto<ClientTokenDTO>.Failure("ClientSecret is wrong", 400, true);
        }

        var token = _tokenService.createTokenByClient(client);

        return ResponseDto<ClientTokenDTO>.Success(token,200);
    }
}