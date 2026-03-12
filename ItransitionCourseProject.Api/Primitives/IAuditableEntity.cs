namespace ItransitionCourseProject.Api.Primitives;

public interface IAuditableEntity
{
    DateTime CreatedOnUtc { get; set; }
    DateTime? ModifiedOnUtc { get; set; }
}