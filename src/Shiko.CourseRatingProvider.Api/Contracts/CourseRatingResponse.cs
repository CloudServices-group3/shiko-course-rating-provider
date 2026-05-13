namespace Shiko.CourseRatingProvider.Api.Contracts;

public sealed record CourseRatingResponse(
    Guid Id,
    Guid CourseId,
    string UserId,
    int Value,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);