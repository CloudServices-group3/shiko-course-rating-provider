using System.ComponentModel.DataAnnotations;

namespace Shiko.CourseRatingProvider.Api.Contracts;

public sealed record UpsertCourseRatingRequest(
    [Range(1, 5)]
    int Value
);