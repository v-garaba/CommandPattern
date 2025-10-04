namespace Command.Validation;

public static class ArgumentValidation
{
    public static T AssertNotNull<T>(this T? obj)
        where T : class
    {
        return obj ?? throw new ArgumentNullException(nameof(obj));
    }

    public static string AssertNotEmpty(this string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentException("String cannot be null or empty.", nameof(str));
        }

        return str;
    }

    public static int AssertPositive(this int number)
    {
        if (number < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "Number must be non-negative.");
        }

        return number;
    }
}
