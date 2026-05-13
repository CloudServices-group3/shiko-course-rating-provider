using System.ComponentModel.DataAnnotations;

namespace Shiko.CourseRatingProvider.Api.Contracts;

public sealed record UpsertCourseRatingRequest(
    [property: Required]
    [property: Range(1, 5)]
    int Value
);