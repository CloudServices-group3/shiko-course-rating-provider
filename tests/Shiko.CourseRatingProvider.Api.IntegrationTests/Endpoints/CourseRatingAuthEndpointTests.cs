using System.Net;
using System.Net.Http.Json;
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
}