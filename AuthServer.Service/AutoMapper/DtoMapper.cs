using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AutoMapper;

namespace AuthServer.Service.AutoMapper;

public class DtoMapper : Profile
{
    public DtoMapper()
    {
        CreateMap<ProductDTO, Product>().ReverseMap();
        CreateMap<UserAppDto, UserApp>().ReverseMap();
    }
}