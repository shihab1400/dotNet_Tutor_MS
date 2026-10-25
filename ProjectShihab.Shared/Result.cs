namespace ProjectShihab.Shared
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public bool HasError { get; set; }
        public string Message { get; set; } = string.Empty;

        public static Result<T> Ok(T? data) => new() { Data = data };

        public static Result<T> Fail(string message) => new() { HasError = true, Message = message };
    }
}
