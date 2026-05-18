# Shiko Course Rating Provider

Small ASP.NET Core Web API provider for course ratings in Shiko LMS.

This provider handles rating values from 1 to 5, average rating, total votes, rating distribution per star level, and one rating per user and course.

Uses EF Core with Azure SQL. Tables are stored in the course_rating schema because my providers share the same Azure SQL database.

Connection strings and JWT settings are stored with user secrets locally and environment variables in Azure.


## Live API

### Base URL

https://shiko-course-rating-provider.azurewebsites.net

The base URL is only the root address for the API. It does not have its own page, so opening it directly can return 404. Use the endpoints below instead.

### Health check

https://shiko-course-rating-provider.azurewebsites.net/health

### Scalar

https://shiko-course-rating-provider.azurewebsites.net/scalar

### OpenAPI JSON

https://shiko-course-rating-provider.azurewebsites.net/openapi/v1.json


## Endpoints

### Public endpoints

These do not require auth because they only return aggregated rating data.

GET /api/course-ratings/{courseId}/summary

POST /api/course-ratings/summaries/batch

The batch endpoint is used so CourseList can fetch ratings for many courses without making one request per course.

### Auth protected endpoints

These require JWT Bearer auth.

GET /api/course-ratings/{courseId}/me

PUT /api/course-ratings/{courseId}/me

DELETE /api/course-ratings/{courseId}/me

The user id is read from the JWT token. Frontend should not send userId in the request body.


## Local config

Set the database connection string with user secrets:

dotnet user-secrets set "ConnectionStrings:CourseRatingDatabase" "your-connection-string" --project .\src\Shiko.CourseRatingProvider.Api

Set JWT config with user secrets:

dotnet user-secrets set "Jwt:Issuer" "your-issuer" --project .\src\Shiko.CourseRatingProvider.Api

dotnet user-secrets set "Jwt:Audience" "your-audience" --project .\src\Shiko.CourseRatingProvider.Api

dotnet user-secrets set "Jwt:SigningKey" "your-signing-key" --project .\src\Shiko.CourseRatingProvider.Api


## Azure config

Azure Web App uses environment variables and app settings.

The database connection string is stored as:

CourseRatingDatabase

JWT app settings are stored as:

Jwt__Issuer

Jwt__Audience

Jwt__SigningKey


## Run locally

Run the API with:

dotnet run --project .\src\Shiko.CourseRatingProvider.Api

Scalar opens at:

https://localhost:your-port/scalar


## Tests

The provider has unit tests and integration tests.

Unit tests cover controller validation.

Integration tests use WebApplicationFactory, Testcontainers, SQL Server container, and EF Core migrations against the test database.

Docker Desktop must be running before running all tests.


## Notes

Courses with no ratings return averageRating = 0 and totalVotes = 0.

Frontend should treat totalVotes === 0 as "No ratings yet", not as an actual 0-star rating.

Summary endpoints are public because they do not return sensitive data.

User-specific rating endpoints are protected because they read and change the logged in user's own rating.
