namespace ItransitionCourseProject.Api.Common;

public record Error(string Message, object? Details)
{
    public static readonly Error None = new("No Error", null);
}