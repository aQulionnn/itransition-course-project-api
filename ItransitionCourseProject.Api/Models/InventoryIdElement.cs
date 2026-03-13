namespace ItransitionCourseProject.Api.Models;

public sealed class InventoryIdElement
{
    public Guid Id { get; set; }
    public ElementType Type { get; set; }
    public int Order { get; set; }
 
    public string? Text { get; set; }
 
    public int Padding { get; set; }
 
    public string? DateFormat { get; set; }
 
    public Guid FormatId { get; set; }
    public required InventoryIdFormat Format { get; set; }
}
 
public enum ElementType
{
    FixedText,
    Random20Bit,
    Random32Bit,
    Random6Digit,
    Random9Digit,
    Guid,
    DateTime,
    Sequence
}