# TemplatePostgreSQL

## About

This template includes:

- A Clean Architecture Web API built with C# / .NET 8.0
- An user CRUD example
- Continuous Integration a.k.a. CI workflow for GitHub Actions
- Dockerfile and docker-compose
- Documented API including Swagger UI
- Integration / Unit tests using the XUnit framework
- JWT authentication / authorization
- Mutation tests with Stryker
- Observability:
    - Logs with Serilog
    - Metrics with OpenTelemetry
    - Traces with OpenTelemetry
- The infrastructure to run on the latest PostgreSQL version
- The infrastructure to run on the latest RabbitMQ version

## Dependencies to execute the application locally

- [.NET 8.0 LTS SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## How to

### Execute the application locally?

In the repository's root directory, execute the command `docker compose up`.

Then, [click here](http://localhost:5001/swagger/index.html) to view the Swagger UI.

Or, execute the command `docker compose up postgres rabbitmq` to run PostgreSQL and RabbitMQ only.

Then, run the application through the IDE and [click here](https://localhost:5001/swagger/index.html) to view the Swagger UI.

Finally, [click here](http://localhost:15672) to manage the RabbitMQ (username and password: guest).

### Execute the application's mutation tests locally?

Execute the command `dotnet tool install -g dotnet-stryker` (one time only).

Then, in the repository's root directory, execute the command `dotnet stryker -s TemplatePostgreSQL.sln`.

### Migrate the database?

Execute the command `dotnet tool install -g dotnet-ef` (one time only).

Then, in the `src` directory, execute the command `dotnet ef -p Infrastructure -s Presentation migrations add MIGRATION_NAME`.

If the application is intended to run through Docker, it will auto migrate. Otherwise, through IDE:

1. Execute the command `docker compose up postgres`
2. Execute the command `dotnet ef -p Infrastructure -s Presentation database update`
3. Run the application through the IDE
