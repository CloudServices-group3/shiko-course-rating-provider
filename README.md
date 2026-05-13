# Shiko Course Rating Provider

Small ASP.NET Core Web API provider for course ratings in Shiko LMS.

Handles:
- rating value 1-5
- average rating
- total votes
- rating distribution per star level

Uses EF Core with Azure SQL. Tables are stored in the course_rating schema because my providers share the same Azure SQL database.

Connection strings are stored with user secrets locally and must not be committed.

No controller/endpoints yet. Protected endpoints will be added after Auth/JWT integration.
