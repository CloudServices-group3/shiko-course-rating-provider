using Microsoft.AspNetCore.Mvc;
using Shiko.CourseRatingProvider.Api.Contracts;
using Shiko.CourseRatingProvider.Api.Services;

namespace Shiko.CourseRatingProvider.Api.Controllers;

[ApiController]
[Route("api/course-ratings")]
public sealed class CourseRatingsController(ICourseRatingService courseRatingService) : ControllerBase
{
    private const int MaxBatchSize = 100; // Define a reasonable maximum batch size to prevent abuse

    [HttpGet("{courseId:guid}/summary")]
    [ProducesResponseType<CourseRatingSummaryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseRatingSummaryResponse>> GetSummary(
        Guid courseId,
        CancellationToken ct)
    {
        if (courseId == Guid.Empty)
        {
            return BadRequest("Course id cannot be empty.");
        }

        var summary = await courseRatingService.GetSummaryAsync(courseId, ct);

        return Ok(summary);
    }

    [HttpPost("summaries/batch")]
    [ProducesResponseType<IReadOnlyList<CourseRatingSummaryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<CourseRatingSummaryResponse>>> GetSummaries(
        [FromBody] CourseRatingSummariesRequest? request,
        CancellationToken ct)
    {
        if (request?.CourseIds is null || request.CourseIds.Count == 0)
        {
            return BadRequest("At least one course id is required.");
        }

        if (request.CourseIds.Count > MaxBatchSize)
        {
            return BadRequest($"A maximum of {MaxBatchSize} course ids is allowed per request.");
        }

        if (request.CourseIds.Any(courseId => courseId == Guid.Empty))
        {
            return BadRequest("Course ids cannot contain empty GUID values.");
        }

        var summaries = await courseRatingService.GetSummariesAsync(request.CourseIds, ct);

        return Ok(summaries);
    }
}