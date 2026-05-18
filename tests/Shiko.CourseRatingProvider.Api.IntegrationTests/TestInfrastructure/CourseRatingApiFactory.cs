using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shiko.CourseRatingProvider.Api.Data;

namespace Shiko.CourseRatingProvider.Api.IntegrationTests.TestInfrastructure;

public sealed class CourseRatingApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public CourseRatingApiFactory(string connectionString)
    {
        _connectionString = connectionString;

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__CourseRatingDatabase",
            connectionString);

        Environment.SetEnvironmentVariable("Jwt__Issuer", "test-issuer");
        Environment.SetEnvironmentVariable("Jwt__Audience", "test-audience");
        Environment.SetEnvironmentVariable(
            "Jwt__SigningKey",
            "test-signing-key-for-integration-tests-123456789");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<CourseRatingDbContext>();
            services.RemoveAll<DbContextOptions<CourseRatingDbContext>>();

            services.AddDbContext<CourseRatingDbContext>(options =>
            {
                options.UseSqlServer(
                    _connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsHistoryTable(
                            "__EFMigrationsHistory",
                            "course_rating");
                    });
            });
        });
    }

    public async Task ApplyMigrationsAsync()
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<CourseRatingDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}