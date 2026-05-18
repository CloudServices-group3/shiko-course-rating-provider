using System.Net;
using Shiko.CourseRatingProvider.Api.IntegrationTests.TestInfrastructure;

namespace Shiko.CourseRatingProvider.Api.IntegrationTests.Endpoints;

public sealed class HealthEndpointTests : IClassFixture<CourseRatingIntegrationTestFixture>
{
    private readonly CourseRatingIntegrationTestFixture _fixture;

    public HealthEndpointTests(CourseRatingIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}