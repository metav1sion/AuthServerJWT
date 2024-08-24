using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.SharedLibrary.Dtos;

namespace AuthServer.Core.Services;

public interface IUserService
{
    Task<ResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<ResponseDto<UserAppDto>> GetUserByUserName(string userName);

}