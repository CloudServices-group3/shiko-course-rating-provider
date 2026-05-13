namespace Shiko.CourseRatingProvider.Api.Models;

public sealed class CourseRating
{
    public Guid Id { get; set; }

    public string CourseId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int Value { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}