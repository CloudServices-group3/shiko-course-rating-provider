using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Shiko.CourseRatingProvider.Api.Models;
using Shiko.CourseRatingProvider.Api.Contracts;
using Shiko.CourseRatingProvider.Api.IntegrationTests.TestInfrastructure;

namespace Shiko.CourseRatingProvider.Api.IntegrationTests.Endpoints;

public sealed class CourseRatingAuthEndpointTests : IClassFixture<CourseRatingIntegrationTestFixture>
{
    private readonly CourseRatingIntegrationTestFixture _fixture;

    public CourseRatingAuthEndpointTests(CourseRatingIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetMyRating_WithoutToken_ReturnsUnauthorized()
    {
        var courseId = Guid.NewGuid();

        var response = await _fixture.Client.GetAsync(
            $"/api/course-ratings/{courseId}/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpsertMyRating_WithoutToken_ReturnsUnauthorized()
    {
        var courseId = Guid.NewGuid();
        var request = new UpsertCourseRatingRequest(5);

        var response = await _fixture.Client.PutAsJsonAsync(
            $"/api/course-ratings/{courseId}/me",
            request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyRating_WithUserTokenAndExistingRating_ReturnsOk()
    {
        var courseId = Guid.NewGuid();
        var userId = "user-1";

        await _fixture.SeedRatingsAsync(new CourseRating
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            UserId = userId,
            Value = 4,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        });

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/course-ratings/{courseId}/me");

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTokenFactory.CreateUserToken(userId));

        var response = await _fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpsertMyRating_WithUserToken_ReturnsOk()
    {
        var courseId = Guid.NewGuid();

        using var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"/api/course-ratings/{courseId}/me");

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTokenFactory.CreateUserToken("user-1"));

        request.Content = JsonContent.Create(new UpsertCourseRatingRequest(5));

        var response = await _fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}