namespace TeamTasks.Domain.Entities
{
    public class Result<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public string? ErrorMessage { get; init; }
        public static Result<T> Success(T data) => new Result<T> { IsSuccess = true, Data = data };
        public static Result<T> Failure(string errorMessage) => new Result<T> { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
