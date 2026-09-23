namespace Application.Dto;

public class ResultGeneric<T>
{
    public T Value { get; }
    public bool IsSuccess { get; }
    public List<string> Errors { get; }

    private ResultGeneric(T value, bool isSuccess, List<string> errors)
    {
        Value = value;
        IsSuccess = isSuccess;
        Errors = errors ?? [];
    }

    public static ResultGeneric<T> Success(T value) => new(value, true, []);

    public static ResultGeneric<T> Failure(List<string> errors) => new(default!, false, errors ?? []);

    public static ResultGeneric<T> Failure(string error) => new(default!, false, [error]);
}
