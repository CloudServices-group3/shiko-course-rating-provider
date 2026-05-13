using Microsoft.EntityFrameworkCore;
using Shiko.CourseRatingProvider.Api.Contracts;
using Shiko.CourseRatingProvider.Api.Data;
using Shiko.CourseRatingProvider.Api.Models;

namespace Shiko.CourseRatingProvider.Api.Services;

public sealed class CourseRatingService(CourseRatingDbContext dbContext) : ICourseRatingService
{
    public async Task<CourseRatingSummaryResponse> GetSummaryAsync(
        Guid courseId,
        CancellationToken ct = default)
    {
        var ratings = await dbContext.CourseRatings
            .AsNoTracking()
            .Where(rating => rating.CourseId == courseId)
            .Select(rating => rating.Value)
            .ToListAsync(ct);

        var totalVotes = ratings.Count;

        var averageRating = totalVotes == 0
            ? 0
            : Math.Round(ratings.Average(), 1);

        var distribution = Enumerable.Range(1, 5)
            .Reverse()
            .Select(stars =>
            {
                var count = ratings.Count(value => value == stars);

                var percentage = totalVotes == 0
                    ? 0
                    : Math.Round((double)count / totalVotes * 100, 1);

                return new CourseRatingDistributionItemResponse(
                    Stars: stars,
                    Percentage: percentage);
            })
            .ToList();

        return new CourseRatingSummaryResponse(
            CourseId: courseId,
            AverageRating: averageRating,
            TotalVotes: totalVotes,
            Distribution: distribution);
    }

    public async Task<CourseRatingResponse?> GetUserRatingAsync(
        Guid courseId,
        string userId,
        CancellationToken ct = default)
    {
        var rating = await dbContext.CourseRatings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                rating => rating.CourseId == courseId && rating.UserId == userId,
                ct);

        return rating is null
            ? null
            : MapToResponse(rating);
    }

    public async Task<CourseRatingResponse> UpsertUserRatingAsync(
        Guid courseId,
        string userId,
        int value,
        CancellationToken ct = default)
    {
        if (value is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Rating value must be between 1 and 5.");
        }

        var rating = await dbContext.CourseRatings
            .FirstOrDefaultAsync(
                rating => rating.CourseId == courseId && rating.UserId == userId,
                ct);

        var now = DateTime.UtcNow;

        if (rating is null)
        {
            rating = new CourseRating
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                UserId = userId,
                Value = value,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            dbContext.CourseRatings.Add(rating);
        }
        else
        {
            rating.Value = value;
            rating.UpdatedAtUtc = now;
        }

        await dbContext.SaveChangesAsync(ct);

        return MapToResponse(rating);
    }

    public async Task<bool> DeleteUserRatingAsync(
        Guid courseId,
        string userId,
        CancellationToken ct = default)
    {
        var rating = await dbContext.CourseRatings
            .FirstOrDefaultAsync(
                rating => rating.CourseId == courseId && rating.UserId == userId,
                ct);

        if (rating is null)
        {
            return false;
        }

        dbContext.CourseRatings.Remove(rating);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    private static CourseRatingResponse MapToResponse(CourseRating rating)
    {
        return new CourseRatingResponse(
            Id: rating.Id,
            CourseId: rating.CourseId,
            UserId: rating.UserId,
            Value: rating.Value,
            CreatedAtUtc: rating.CreatedAtUtc,
            UpdatedAtUtc: rating.UpdatedAtUtc);
    }
}