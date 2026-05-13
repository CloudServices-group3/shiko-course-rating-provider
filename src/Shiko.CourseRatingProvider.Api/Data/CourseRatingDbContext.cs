using Microsoft.EntityFrameworkCore;
using Shiko.CourseRatingProvider.Api.Models;

namespace Shiko.CourseRatingProvider.Api.Data;

public sealed class CourseRatingDbContext(DbContextOptions<CourseRatingDbContext> options)
    : DbContext(options)
{
    public DbSet<CourseRating> CourseRatings => Set<CourseRating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseRatingDbContext).Assembly);
    }
}