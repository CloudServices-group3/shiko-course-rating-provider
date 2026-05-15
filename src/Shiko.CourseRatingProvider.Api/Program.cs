using Shiko.CourseRatingProvider.Api.Services;
using Shiko.CourseRatingProvider.Api.Data;
using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<ICourseRatingService, CourseRatingService>();
builder.Services.AddDbContext<CourseRatingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CourseRatingDatabase"));
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Shiko Course Rating Provider API");
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/health", () =>
    Results.Ok(new
    {
        status = "Healthy",
        service = "Shiko Course Rating Provider",
        utc = DateTime.UtcNow
    }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();