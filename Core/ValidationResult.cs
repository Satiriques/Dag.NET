namespace Core
{
    public class ValidationResult
    {
        public bool Successful { get; init; }
        public string Message { get; init; }
        public static ValidationResult Success { get; } = new() {Message = null, Successful = true};
    }
}