using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.Services;
using AuthServer.Service.AutoMapper;
using AuthServer.SharedLibrary.Dtos;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Service.Services;

public class UserService : IUserService
{
    private readonly UserManager<UserApp> _userManager;

    public UserService(UserManager<UserApp> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new UserApp
        {
            UserName = createUserDto.UserName,
            Email = createUserDto.Email
        };

        var result = await _userManager.CreateAsync(user,createUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x=>x.Description).ToList();
            return ResponseDto<UserAppDto>.Failure(new ErrorDto(errors,true),400);
        }
        return ResponseDto<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 400);

    }

    public async Task<ResponseDto<UserAppDto>> GetUserByUserName(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user==null)
        {
            return ResponseDto<UserAppDto>.Failure("Username Not Found!",404,true);
        }
        return ResponseDto<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user),200);
    }
}