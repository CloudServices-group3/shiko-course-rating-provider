namespace Shiko.CourseRatingProvider.Api.Contracts;

public sealed record CourseRatingDistributionItemResponse(
    int Stars,
    double Percentage
);