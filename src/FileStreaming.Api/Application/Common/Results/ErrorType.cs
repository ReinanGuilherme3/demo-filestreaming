namespace FileStreaming.Api.Application.Common.Results;

// Cada tipo corresponde a um status HTTP (ver ResultExtensions na camada Api)
public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict,
    Gone
}
