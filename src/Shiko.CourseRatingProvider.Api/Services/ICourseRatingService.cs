using Shiko.CourseRatingProvider.Api.Contracts;

namespace Shiko.CourseRatingProvider.Api.Services;

public interface ICourseRatingService
{
    Task<CourseRatingSummaryResponse> GetSummaryAsync(
        Guid courseId,
        CancellationToken ct = default);

    Task<CourseRatingResponse?> GetUserRatingAsync(
        Guid courseId,
        string userId,
        CancellationToken ct = default);

    Task<CourseRatingResponse> UpsertUserRatingAsync(
        Guid courseId,
        string userId,
        int value,
        CancellationToken ct = default);

    Task<bool> DeleteUserRatingAsync(
        Guid courseId,
        string userId,
        CancellationToken ct = default);
}