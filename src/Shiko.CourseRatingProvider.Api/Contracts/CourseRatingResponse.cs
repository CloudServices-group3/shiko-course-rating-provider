namespace Shiko.CourseRatingProvider.Api.Contracts;

public sealed record CourseRatingResponse(
    Guid Id,
    string CourseId,
    string UserId,
    int Value,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);