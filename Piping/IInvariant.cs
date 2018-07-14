using System.ComponentModel.DataAnnotations;

namespace Piping
{
    public interface IInvariant<out T>
    {
        bool IsValid { get; }
        T SetError(string error);
        string Error { get; }
    }

    public static class ValidationResultExt
    {
        public static ValidationResult ToValidationResult<T>(this IInvariant<T> source) => new ValidationResult(source.Error);
    }
}
