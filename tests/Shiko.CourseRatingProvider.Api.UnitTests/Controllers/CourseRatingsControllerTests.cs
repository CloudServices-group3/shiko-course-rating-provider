using Microsoft.AspNetCore.Mvc;
using Shiko.CourseRatingProvider.Api.Contracts;
using Shiko.CourseRatingProvider.Api.Controllers;
using Shiko.CourseRatingProvider.Api.Services;

namespace Shiko.CourseRatingProvider.Api.UnitTests.Controllers;

public sealed class CourseRatingsControllerTests
{
    [Fact]
    public async Task GetSummary_WhenCourseIdIsEmpty_ReturnsBadRequest()
    {
        var controller = CreateController();

        var result = await controller.GetSummary(Guid.Empty, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Course id cannot be empty.", badRequest.Value);
    }

    [Fact]
    public async Task GetSummaries_WhenRequestIsNull_ReturnsBadRequest()
    {
        var controller = CreateController();

        var result = await controller.GetSummaries(null, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("At least one course id is required.", badRequest.Value);
    }

    [Fact]
    public async Task GetSummaries_WhenCourseIdsIsEmpty_ReturnsBadRequest()
    {
        var controller = CreateController();
        var request = new CourseRatingSummariesRequest([]);

        var result = await controller.GetSummaries(request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("At least one course id is required.", badRequest.Value);
    }

    [Fact]
    public async Task GetSummaries_WhenCourseIdsContainsEmptyGuid_ReturnsBadRequest()
    {
        var controller = CreateController();
        var request = new CourseRatingSummariesRequest([Guid.NewGuid(), Guid.Empty]);

        var result = await controller.GetSummaries(request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Course ids cannot contain empty GUID values.", badRequest.Value);
    }

    [Fact]
    public async Task GetSummaries_WhenCourseIdsExceedsMaxBatchSize_ReturnsBadRequest()
    {
        var controller = CreateController();
        var courseIds = Enumerable
            .Range(0, 101)
            .Select(_ => Guid.NewGuid())
            .ToList();

        var request = new CourseRatingSummariesRequest(courseIds);

        var result = await controller.GetSummaries(request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("A maximum of 100 course ids is allowed per request.", badRequest.Value);
    }

    [Fact]
    public async Task UpsertMyRating_WhenCourseIdIsEmpty_ReturnsBadRequest()
    {
        var controller = CreateController();
        var request = new UpsertCourseRatingRequest(5);

        var result = await controller.UpsertMyRating(Guid.Empty, request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Course id cannot be empty.", badRequest.Value);
    }

    [Fact]
    public async Task UpsertMyRating_WhenRequestIsNull_ReturnsBadRequest()
    {
        var controller = CreateController();

        var result = await controller.UpsertMyRating(Guid.NewGuid(), null, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Request body is required.", badRequest.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task UpsertMyRating_WhenRatingValueIsOutsideAllowedRange_ReturnsBadRequest(int value)
    {
        var controller = CreateController();
        var request = new UpsertCourseRatingRequest(value);

        var result = await controller.UpsertMyRating(Guid.NewGuid(), request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Rating value must be between 1 and 5.", badRequest.Value);
    }

    private static CourseRatingsController CreateController()
    {
        return new CourseRatingsController(new ThrowingCourseRatingService());
    }

    private sealed class ThrowingCourseRatingService : ICourseRatingService
    {
        public Task<CourseRatingSummaryResponse> GetSummaryAsync(
            Guid courseId,
            CancellationToken ct = default)
        {
            throw new InvalidOperationException("Service should not be called for validation failures.");
        }

        public Task<IReadOnlyList<CourseRatingSummaryResponse>> GetSummariesAsync(
            IReadOnlyCollection<Guid> courseIds,
            CancellationToken ct = default)
        {
            throw new InvalidOperationException("Service should not be called for validation failures.");
        }

        public Task<CourseRatingResponse?> GetUserRatingAsync(
            Guid courseId,
            string userId,
            CancellationToken ct = default)
        {
            throw new InvalidOperationException("Service should not be called for validation failures.");
        }

        public Task<CourseRatingResponse> UpsertUserRatingAsync(
            Guid courseId,
            string userId,
            int value,
            CancellationToken ct = default)
        {
            throw new InvalidOperationException("Service should not be called for validation failures.");
        }

        public Task<bool> DeleteUserRatingAsync(
            Guid courseId,
            string userId,
            CancellationToken ct = default)
        {
            throw new InvalidOperationException("Service should not be called for validation failures.");
        }
    }
}