using System.Collections;
using System.Linq.Expressions;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using AuthServer.Service.AutoMapper;
using AuthServer.SharedLibrary.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Service.Services;

public class GenericService<TEntity,TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<TEntity> _genericRepository;

    public GenericService(IGenericRepository<TEntity> genericRepository, IUnitOfWork unitOfWork)
    {
        _genericRepository = genericRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<TDto>> GetByIdAsync(int id)
    {
        var entityValue = await _genericRepository.GetByIdAsync(id);
        if (entityValue == null)
        {
            return ResponseDto<TDto>.Failure("Id Not Found",404,true);
        }
        var dtoValue = ObjectMapper.Mapper.Map<TDto>(entityValue);
        return ResponseDto<TDto>.Success(dtoValue, 200);
    }

    public async Task<ResponseDto<IEnumerable<TDto>>> GetAllAsync()
    {
        var values = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());
        return ResponseDto<IEnumerable<TDto>>.Success(values, 200);
    }

    public async Task<ResponseDto<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
    {
        var values = await _genericRepository.Where(predicate).ToListAsync();
        var dtoValues = ObjectMapper.Mapper.Map<List<TDto>>(values);
        return ResponseDto<IEnumerable<TDto>>.Success(dtoValues,200);
    }

    public async Task<ResponseDto<TDto>> AddAsync(TDto dto)
    {
        var newEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
        await _genericRepository.AddAsync(newEntity);

        await _unitOfWork.CommitAsync();

        var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
        return ResponseDto<TDto>.Success(newDto, 200);

    }

    public async Task<ResponseDto<NoDataDTO>> Remove(int id)
    {
        var entity = await _genericRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return ResponseDto<NoDataDTO>.Failure("Id Not Found", 404, true);
        }

        // Varlığı kaldırma
        _genericRepository.Remove(entity);
        await _unitOfWork.CommitAsync();

        return ResponseDto<NoDataDTO>.Success(204);
    }

    public async Task<ResponseDto<NoDataDTO>> Update(TDto dto, int id)
    {

        // ID'ye göre mevcut varlığı repository'den al
        var existingEntity = await _genericRepository.GetByIdAsync(id); //memoryde track edilmiyor
        if (existingEntity == null)
        {
            return ResponseDto<NoDataDTO>.Failure("Id Not Found", 404, true);
        }

        // DTO'dan yeni varlığı oluştur
        var updatedEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
        _genericRepository.Update(updatedEntity);
        await _unitOfWork.CommitAsync();

        return ResponseDto<NoDataDTO>.Success(new NoDataDTO(), 204);
    }
}