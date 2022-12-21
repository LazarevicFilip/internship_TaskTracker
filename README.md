# internship_TaskTracker

## Guideline

### Description
- This application represent N-Layer architecture style.(Domain, BL, DAL, PL). It is builded around two entities - Project and Tasks.
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


 
