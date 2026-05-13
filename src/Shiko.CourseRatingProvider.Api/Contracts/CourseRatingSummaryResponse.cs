namespace Shiko.CourseRatingProvider.Api.Contracts;

public sealed record CourseRatingSummaryResponse(
    Guid CourseId,
    double AverageRating,
    int TotalVotes,
    IReadOnlyList<CourseRatingDistributionItemResponse> Distribution
);