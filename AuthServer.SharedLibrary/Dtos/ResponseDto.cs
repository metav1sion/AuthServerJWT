using System.Text.Json.Serialization;
using Microsoft.VisualBasic;

namespace AuthServer.SharedLibrary.Dtos;

public class ResponseDto<T> where T : class
{
    public T Data { get; private set; }
    public int StatusCode { get; private set; }
    public ErrorDto Error { get; private set; }

    [JsonIgnore]
    public bool IsSuccessful { get; private set; }

    public static ResponseDto<T> Success(T data, int statusCode)
    {
        return new ResponseDto<T>
        {
            Data = data,
            StatusCode = statusCode,
            IsSuccessful = true
        };
    }
    public static ResponseDto<T> Success(int statusCode)
    {
        return new ResponseDto<T>
        {
            Data = default,
            StatusCode = statusCode,
            IsSuccessful = true
        };
    }

    public static ResponseDto<T> Failure(ErrorDto errorDto, int statusCode) //birden fazla hata geldiğinde
    {
        return new ResponseDto<T>
        {
            StatusCode = statusCode,
            Error = errorDto,
            IsSuccessful = false
        };
    }
    public static ResponseDto<T> Failure(string errorMessage, int statusCode, bool isShow) // tek hata geldiğinde
    {
        var errorDto = new ErrorDto(errorMessage,isShow);
        return new ResponseDto<T>
        {
            StatusCode = statusCode,
            Error = errorDto,
            IsSuccessful = false
        };
    }



}