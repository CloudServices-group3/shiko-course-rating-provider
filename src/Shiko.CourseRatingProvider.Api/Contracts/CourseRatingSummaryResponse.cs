namespace Shiko.CourseRatingProvider.Api.Contracts;

public sealed record CourseRatingSummaryResponse(
    string CourseId,
    double AverageRating,
    int TotalVotes,
    IReadOnlyList<CourseRatingDistributionItemResponse> Distribution
);