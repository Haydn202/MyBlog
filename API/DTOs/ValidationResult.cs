namespace API.DTOs;

public class ValidationResult<T>
{
    public T? Data { get; set; }
    public bool IsSuccess => Errors == null || !Errors.Any();
    public List<string> Errors { get; set; } = new();

    public static ValidationResult<T> Success(T data) => new ValidationResult<T> { Data = data };
    public static ValidationResult<T> Failure(List<string> errors) => new ValidationResult<T> { Errors = errors };
}
