namespace Shiko.CourseRatingProvider.Api.Contracts;

public sealed record CourseRatingSummariesRequest(
    IReadOnlyList<Guid> CourseIds
);