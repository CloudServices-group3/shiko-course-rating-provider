using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shiko.CourseRatingProvider.Api.Data;
using Shiko.CourseRatingProvider.Api.Models;
using Testcontainers.MsSql;

namespace Shiko.CourseRatingProvider.Api.IntegrationTests.TestInfrastructure;

public sealed class CourseRatingIntegrationTestFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder(
        "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
        .Build();

    private CourseRatingApiFactory? _factory;

    public HttpClient Client { get; private set; } = null!;

    public IServiceProvider Services => _factory!.Services;

    public async Task InitializeAsync()
    {
        await _sqlServer.StartAsync();

        _factory = new CourseRatingApiFactory(_sqlServer.GetConnectionString());

        await _factory.ApplyMigrationsAsync();

        Client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        _factory?.Dispose();

        await _sqlServer.DisposeAsync();
    }

    public async Task SeedRatingsAsync(params CourseRating[] ratings)
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<CourseRatingDbContext>();

        dbContext.CourseRatings.AddRange(ratings);

        await dbContext.SaveChangesAsync();
    }
}