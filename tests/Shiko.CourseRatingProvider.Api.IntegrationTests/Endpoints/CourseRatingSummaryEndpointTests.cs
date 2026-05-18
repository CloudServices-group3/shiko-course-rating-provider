using System.Net;
using System.Net.Http.Json;
using Shiko.CourseRatingProvider.Api.Contracts;
using Shiko.CourseRatingProvider.Api.IntegrationTests.TestInfrastructure;
using Shiko.CourseRatingProvider.Api.Models;

namespace Shiko.CourseRatingProvider.Api.IntegrationTests.Endpoints;

public sealed class CourseRatingSummaryEndpointTests : IClassFixture<CourseRatingIntegrationTestFixture>
{
    private readonly CourseRatingIntegrationTestFixture _fixture;

    public CourseRatingSummaryEndpointTests(CourseRatingIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetSummary_WhenCourseHasNoRatings_ReturnsZeroAverageAndZeroVotes()
    {
        var courseId = Guid.NewGuid();

        var response = await _fixture.Client.GetAsync(
            $"/api/course-ratings/{courseId}/summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var summary = await response.Content
            .ReadFromJsonAsync<CourseRatingSummaryResponse>();

        Assert.NotNull(summary);
        Assert.Equal(courseId, summary.CourseId);
        Assert.Equal(0, summary.AverageRating);
        Assert.Equal(0, summary.TotalVotes);
        Assert.Equal([5, 4, 3, 2, 1], summary.Distribution.Select(item => item.Stars));
        Assert.All(summary.Distribution, item => Assert.Equal(0, item.Percentage));
    }

    [Fact]
    public async Task GetSummary_WhenCourseHasRatings_ReturnsAverageTotalVotesAndDistribution()
    {
        var courseId = Guid.NewGuid();

        await _fixture.SeedRatingsAsync(
            CreateRating(courseId, "user-1", 5),
            CreateRating(courseId, "user-2", 4),
            CreateRating(courseId, "user-3", 4),
            CreateRating(courseId, "user-4", 1));

        var response = await _fixture.Client.GetAsync(
            $"/api/course-ratings/{courseId}/summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var summary = await response.Content
            .ReadFromJsonAsync<CourseRatingSummaryResponse>();

        Assert.NotNull(summary);
        Assert.Equal(courseId, summary.CourseId);
        Assert.Equal(3.5, summary.AverageRating);
        Assert.Equal(4, summary.TotalVotes);

        var distribution = summary.Distribution.ToDictionary(
            item => item.Stars,
            item => item.Percentage);

        Assert.Equal(25.0, distribution[5]);
        Assert.Equal(50.0, distribution[4]);
        Assert.Equal(0.0, distribution[3]);
        Assert.Equal(0.0, distribution[2]);
        Assert.Equal(25.0, distribution[1]);
    }

    [Fact]
    public async Task GetSummaries_WhenMultipleCourseIdsAreRequested_ReturnsSummariesForEachCourse()
    {
        var firstCourseId = Guid.NewGuid();
        var secondCourseId = Guid.NewGuid();

        await _fixture.SeedRatingsAsync(
            CreateRating(firstCourseId, "user-1", 5),
            CreateRating(firstCourseId, "user-2", 3),
            CreateRating(secondCourseId, "user-3", 4));

        var request = new CourseRatingSummariesRequest(
        [
            firstCourseId,
        secondCourseId
        ]);

        var response = await _fixture.Client.PostAsJsonAsync(
            "/api/course-ratings/summaries/batch",
            request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var summaries = await response.Content
            .ReadFromJsonAsync<List<CourseRatingSummaryResponse>>();

        Assert.NotNull(summaries);
        Assert.Equal(2, summaries.Count);

        var firstSummary = Assert.Single(
            summaries,
            summary => summary.CourseId == firstCourseId);

        Assert.Equal(4.0, firstSummary.AverageRating);
        Assert.Equal(2, firstSummary.TotalVotes);

        var secondSummary = Assert.Single(
            summaries,
            summary => summary.CourseId == secondCourseId);

        Assert.Equal(4.0, secondSummary.AverageRating);
        Assert.Equal(1, secondSummary.TotalVotes);
    }

    private static CourseRating CreateRating(
        Guid courseId,
        string userId,
        int value)
    {
        var now = DateTime.UtcNow;

        return new CourseRating
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
            Value = value,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
    }
}