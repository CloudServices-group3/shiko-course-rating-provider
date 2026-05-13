using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shiko.CourseRatingProvider.Api.Models;

namespace Shiko.CourseRatingProvider.Api.Data.Configurations;

public sealed class CourseRatingConfiguration : IEntityTypeConfiguration<CourseRating>
{
    public void Configure(EntityTypeBuilder<CourseRating> builder)
    {
        builder.ToTable("CourseRatings", "course_rating", table =>
        {
            table.HasCheckConstraint(
                "CK_CourseRatings_Value",
                "[Value] BETWEEN 1 AND 5");
        });

        builder.HasKey(rating => rating.Id);

        builder.Property(rating => rating.CourseId)
            .IsRequired();

        builder.Property(rating => rating.UserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(rating => rating.Value)
            .IsRequired();

        builder.Property(rating => rating.CreatedAtUtc)
            .IsRequired();

        builder.Property(rating => rating.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(rating => new
        {
            rating.CourseId,
            rating.UserId
        })
            .IsUnique();
    }
}