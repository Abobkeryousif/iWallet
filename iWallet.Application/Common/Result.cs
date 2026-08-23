
namespace iWallet.Application.Common
{
    public enum ErrorType
    {
        Validation = 1,
        NotFound,
        Conflict,
       Failure
    }

    public sealed record ServiceError(
        string code, 
        string message,
        ErrorType type);


    public sealed class Result<T>
    {
        private Result(T? value, ServiceError? error)
        {
            Value = value;
            Error = error;

        }

        public T? Value { get; }
        public ServiceError? Error { get; }

        public bool IsSuccess => Error is null;

        public static Result<T> Success(T value) =>
            new(value,null);

        public static Result<T> Failure(ServiceError error) =>
            new(default,error);
    }
}






