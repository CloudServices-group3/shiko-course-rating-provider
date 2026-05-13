namespace Shiko.CourseRatingProvider.Api.Models;

public sealed class CourseRating
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string UserId { get; set; } = null!;

    public int Value { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}