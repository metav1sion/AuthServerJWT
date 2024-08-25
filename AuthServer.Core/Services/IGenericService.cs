using System.Linq.Expressions;
using AuthServer.SharedLibrary.Dtos;

namespace AuthServer.Core.Services;

public interface IGenericService<TEntity,TDto> where TEntity : class where TDto : class
{
    Task<ResponseDto<TDto>> GetByIdAsync(int id);
    Task<ResponseDto<IEnumerable<TDto>>> GetAllAsync();
    Task<ResponseDto<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate); //TEntity alan ve bool dönen bir metot girilecek içine.
    Task<ResponseDto<TDto>> AddAsync(TDto dto);
    Task<ResponseDto<NoDataDTO>> Remove(int id);
    Task<ResponseDto<NoDataDTO>> Update(TDto dto, int id);
}