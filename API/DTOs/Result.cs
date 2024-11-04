namespace API.DTOs;

public class Result<T>
{
    public T? Data { get; set; }
    public bool IsSuccess => Errors == null || !Errors.Any();
    public List<string> Errors { get; set; } = new();

    public static Result<T> Success(T data) => new Result<T> { Data = data };
    public static Result<T> Failure(List<string> errors) => new Result<T> { Errors = errors };
}
