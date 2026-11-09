# RaceDay – Part 3: MVC Web Application
**Student:** Natanael Young  
**Student number:** ST10466127  


## Overview
Part 3 adds an ASP.NET Core MVC front end to the RaceDay API from Part 2. The MVC application communicates with the API over HTTP and does not contain Entity Framework, SQL connections or direct database operations. The API remains responsible for business rules, persistence, authentication and all image storage.

## Technology
- ASP.NET Core MVC / .NET 8
- Razor Views and Bootstrap 5
- `HttpClient` API service
- ASP.NET Core Session for the MVC user's signed-in state
- Docker multi-stage build
- GitHub Actions build workflow

## MVC structure
- **Models:** View models used to validate form input.
- **Views:** Razor pages that render the organiser and participant interfaces.
- **Controllers:** Receive browser requests and call `RaceDayApiClient`.
- **Services/RaceDayApiClient:** Centralises REST API calls and forwards the API bearer token.
- **Services/SessionAuth:** Stores token, role, display name and email in server-side session.

## Configure the Part 2 API
1. Open `RaceDay.Mvc/appsettings.json`.
2. Set `Api:BaseUrl` to the running Part 2 API address, including its trailing slash, e.g. `https://localhost:7001/`.
3. Run the Part 2 API before starting the MVC site.
4. The current client assumes these REST routes. **Compare them with your actual Part 2 controller routes and update the paths in the MVC controllers if your names differ.**

| Feature | Expected API route |
|---|---|
| Login | `POST api/auth/login` |
| Register | `POST api/auth/register` |
| Public event list | `GET api/events` |
| Organiser's events | `GET api/events/mine` |
| Event details | `GET api/events/{id}` |
| Create/update/delete event | `POST/PUT/DELETE api/events[/{id}]` |
| Event banner upload | `POST api/events/{id}/banner` (multipart field `file`) |
| Event categories | `GET/POST api/events/{id}/categories` |
| Event enrolments | `GET api/events/{id}/enrolments` |
| Enrolment create | `POST api/enrolments` |
| My enrolments | `GET api/enrolments/mine` |
| Result entry | `POST api/enrolments/{id}/result` |
| My results | `GET api/results/mine` |
| Profile read/update | `GET/PUT api/users/me` |
| Profile picture | `POST api/users/me/profile-picture` (multipart field `file`) |

The login JSON response must include a bearer token (`token`, `accessToken` or `jwt`) and a user object or top-level `role`/`name`/`email`. Update `SessionAuth` if Part 2 returns a different shape. Register currently sends `Name`, `Email`, `Password` and `Role`.

## Azure Blob Storage requirement
The MVC project deliberately does **not** connect to Azure. The event banner and profile-picture forms send multipart requests to the Part 2 API. The API must validate image type/size, upload to a private or appropriately configured Azure Blob container, save the resulting blob URL/reference, and return it in its event/profile DTO. The API should also enforce organiser/participant ownership and authorisation. Configure Azure connection details only in the API's user secrets/environment variables, never in this MVC repository.

## Run locally
```bash
cd RaceDay.Mvc
dotnet restore
dotnet run
```
Browse to the HTTPS/HTTP URL printed by ASP.NET Core.

## Docker
From the repository root:
```bash
docker build -t raceday-mvc:latest -f RaceDay.Mvc/Dockerfile RaceDay.Mvc
docker run --rm -p 8080:8080 -e Api__BaseUrl="http://host.docker.internal:5000/" --name raceday-mvc raceday-mvc:latest
```
On Linux, if the API is running on the host, add `--add-host=host.docker.internal:host-gateway`. If the API is another Docker container, put both containers on the same Docker network and use the API container/service name as the base URL. The API URL must be reachable from inside the MVC container.




