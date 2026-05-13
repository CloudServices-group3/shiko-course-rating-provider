using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiko.CourseRatingProvider.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "course_rating");

            migrationBuilder.CreateTable(
                name: "CourseRatings",
                schema: "course_rating",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseRatings", x => x.Id);
                    table.CheckConstraint("CK_CourseRatings_Value", "[Value] BETWEEN 1 AND 5");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseRatings_CourseId_UserId",
                schema: "course_rating",
                table: "CourseRatings",
                columns: new[] { "CourseId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseRatings",
                schema: "course_rating");
        }
    }
}
