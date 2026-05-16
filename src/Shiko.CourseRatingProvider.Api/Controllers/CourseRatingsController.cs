using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shiko.CourseRatingProvider.Api.Contracts;
using Shiko.CourseRatingProvider.Api.Services;

namespace Shiko.CourseRatingProvider.Api.Controllers;

[ApiController]
[Route("api/course-ratings")]
public sealed class CourseRatingsController(ICourseRatingService courseRatingService) : ControllerBase
{
    private const int MaxBatchSize = 100; // Max batch size to prevent abuse.

    [AllowAnonymous]
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

    [AllowAnonymous]
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

    [Authorize]
    [HttpGet("{courseId:guid}/me")]
    [ProducesResponseType<CourseRatingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseRatingResponse>> GetMyRating(
        Guid courseId,
        CancellationToken ct)
    {
        if (courseId == Guid.Empty)
        {
            return BadRequest("Course id cannot be empty.");
        }

        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var rating = await courseRatingService.GetUserRatingAsync(courseId, userId, ct);

        if (rating is null)
        {
            return NotFound();
        }

        return Ok(rating);
    }

    [Authorize]
    [HttpPut("{courseId:guid}/me")]
    [ProducesResponseType<CourseRatingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CourseRatingResponse>> UpsertMyRating(
        Guid courseId,
        [FromBody] UpsertCourseRatingRequest? request,
        CancellationToken ct)
    {
        if (courseId == Guid.Empty)
        {
            return BadRequest("Course id cannot be empty.");
        }

        if (request is null)
        {
            return BadRequest("Request body is required.");
        }

        if (request.Value is < 1 or > 5)
        {
            return BadRequest("Rating value must be between 1 and 5.");
        }

        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var rating = await courseRatingService.UpsertUserRatingAsync(
            courseId,
            userId,
            request.Value,
            ct);

        return Ok(rating);
    }

    [Authorize]
    [HttpDelete("{courseId:guid}/me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMyRating(
        Guid courseId,
        CancellationToken ct)
    {
        if (courseId == Guid.Empty)
        {
            return BadRequest("Course id cannot be empty.");
        }

        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await courseRatingService.DeleteUserRatingAsync(courseId, userId, ct);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private string? GetUserId()
    {
        return User.FindFirst("userId")?.Value;
    }
}