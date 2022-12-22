# internship_TaskTracker

## Guideline

### Description
- Application is responsible for managing tasks and projects. It is built around two entities - Project and Tasks. The application support CRUD operations on entities and also operation for multiple deleting and inserting tasks for project.
- This application represent N-Layer architecture style.(Domain, BL, DAL, PL).
- Some endpoints are protected with JWT Bearer authentication scheme and expects tokens. 
- Application heavy adhering to repository and unit of work patterns. With goal to abstract and encapsulate business logic and data access logic from controllers in presentation layer.
### Install Db
- Create database with arbitrary name in Microsoft SQL server.
- Change connection string in appsettings.json to suit on your environment. (dbname, dbserver)
- Run migration (Update-Database command). Set DataAccess.DAL as a starting project.
### Creating a user
- POST /api/auth/register with valid JSON - creates new user.
- POST /api/auth with valid JSON - login user and returns JWT token.
- With this token has the possibility to access protected routes.
- For list of all endpoints, see Swagger file.
### Third-party frameworks and packages
- Microsoft.EntityFrameworkCore Version=6.0.11 + (design,tools,sqlServer)
- BCrypt.Net-Next Version=4.0.3
- Microsoft.AspNetCore.Authentication.JwtBearer Version=6.0.11
- Swashbuckle.AspNetCore Version=6.2.3
- FluentValidation Version=11.4.0

 
