using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Shared.Helpers;
public class ResponseData<T>
{
    public T? Result { get; set; }

    public ResponseError? Error { get; set; }

    public ResponseData(T result)
    {
        Result = result;
    }

    public ResponseData(string error)
    {

        Error = new ResponseError
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = error
        };
    }
    public ResponseData(string error, HttpStatusCode statusCode)
    {

        Error = new ResponseError
        {
            StatusCode = (int)statusCode,
            Message = error
        };
    }
    public ResponseData()
    {
    }

    public ResponseData(ModelStateDictionary modelState)
    {
       
        var errorList = modelState.Keys.SelectMany(key => modelState[key]?.Errors.Select(x => key + ": " + x.ErrorMessage) ?? Array.Empty<string>()).ToList();

        Error = new ResponseError
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = string.Join(" | ", errorList)
        };
    }

    public ResponseData(Exception ex)
    {
        Error = new ResponseError
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = ex.Message + "; " + ex.InnerException?.Message
        };
    }

    public ResponseData(HttpStatusCode enumStatusCode)
    {
        int statusCode = (int)enumStatusCode;

        if (statusCode is < 200 or > 300)
        {
            Error = enumStatusCode switch
            {
                HttpStatusCode.Unauthorized => new ResponseError { StatusCode = (int)statusCode, Message = "Не авторизован" },
                HttpStatusCode.BadRequest => new ResponseError { StatusCode = (int)statusCode, Message = "Проверьте правильность передаюмые параметры" },
                HttpStatusCode.Forbidden => new ResponseError { StatusCode = (int)statusCode, Message = "Доступ запрещён" },
                HttpStatusCode.NotFound => new ResponseError { StatusCode = (int)statusCode, Message = "Не найдено!" },
                HttpStatusCode.InternalServerError => new ResponseError { StatusCode = (int)statusCode, Message = "Внутренняя ошибка сервера" },
                _ => throw new ArgumentOutOfRangeException(nameof(statusCode), statusCode, "Invalid status code")
            };
        }

        if (enumStatusCode == HttpStatusCode.OK)
        {
            Result = (T)(object)true;
        }
    }

    public static implicit operator ResponseData<T>(T value) => new(value);

    public static implicit operator ResponseData<T>(string error) => new(error);

    public static implicit operator ResponseData<T>(ModelStateDictionary modelState) => new(modelState);

    public static implicit operator ResponseData<T>(Exception ex) => new(ex);

    public static implicit operator ResponseData<T>(HttpStatusCode code) => new(code);
}
