namespace Trilha.DotNet.Shared.Contracts;

public class Result<T>
{
    private T? Value { get; }
    private ResultError? Error { get; }

    public bool IsSuccess => Error == null;

    public Result(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Error = null;
    }

    public Result(ResultError error)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));
        Value = default;
    }

    public static implicit operator Result<T>(T value) => new(value);
    public static implicit operator Result<T>(ResultError error) => new(error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<ResultError, TResult> onFailure)
    {
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }

    public T EnsureValue()
    {
        if (Error != null)
            throw new InvalidOperationException("Cannot access value because there was an error.");

        return Value!;
    }

    public ResultError EnsureError()
    {
        if (Error == null)
            throw new InvalidOperationException("Cannot access error because the result is successful.");

        return Error!;
    }

    public IActionResult CheckResult(HttpStatusCode successStatusCode = HttpStatusCode.OK)
    {
        if (Error == null)
            return new ObjectResult(Value)
            {
                StatusCode = (int)successStatusCode
            };

        return new ObjectResult(Error.Messages)
        {
            StatusCode = (int)Error.HttpStatusCode
        };
    }
}
