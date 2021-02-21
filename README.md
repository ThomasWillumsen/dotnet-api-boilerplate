# REST API Boilerplate

Everything in this repo is meant as a boilerplate to copy when starting a new API project. Even the rest of this readme file is example-text.

## About this project

This is a boilerplate for an API.

Hosted at:

- https://dev-mywebsite.azurewebsites.net
- https://test-mywebsite.azurewebsites.net
- https://mywebsite.azurewebsites.net

**Highlights:**

- It's a .Net 5 app written in C#
- The API is documented using Swagger UI at `/swagger/index.html`. Clients can make use of Swagger Codegen.
- The project administers an SQL Server database using Entity Framework and the code-first principle with migrations.
- The solution is divided into two separate projects; the API project responsible for the interface of the application and http communication with clients, and the Core project which holds business logic and communication with external resources.

### Prerequisites

- .Net SDK 5.0.1

## Getting Started

### Environment variables

Please make sure that you have set the following environment variables when running the project locally.
Using VS Code, a launch.json file can have a configuration containing an env object as such:

```
"env": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "ConnectionStrings:DbConnection": ""
}
```

Connectionstrings and other secret stuff can be found in the Azure Portal in the app service configurations or in a Key Vault.
Depending on the `ASPNETCORE_ENVIRONMENT` variable, an additional appsettings.{env}.json file will be read, which contains additional environment-specific configuration.

## Entity Framework

Migrations are applied when the application starts and should generally not be applied manually using the command line.
This means that upon deploying the app, migrations will automatically update the database.
it is important to configure the connection string in appsettings.json temporarily.
While you are working with a new database scheme you should **not** connect to the Test or Production database.
It should point at a local database running on your machine so you dont accidentally mess up the database scheme on Test or Production.
I use the following connection string to point at a locally running LocalDb, which can be installed as part of the SQL Server Express installation. `Server=(localdb)\\MSSQLLocalDB;Initial Catalog=OperationDashboard-local;Integrated Security=true"`
Connect to the local database in SSMS using server name: (LocalDB)\MSSQLLocalDB.

To add new migrations

```sh
cd ./src/Boilerplate.Core/
dotnet ef migrations add InitialCreate -o ./Database/Migrations/
```

## Run the app

Using Dotnet

```sh
dotnet run --project ./src/Boilerplate.Api/
```

## Tests

We're using Xunit and Moq as mocking framework.
The general strategy for testing is the following:

- Unit test all business logic
- Integration test all database integrations with in-memory database
- Mock/Stub external services. No integrations with external services are tested.
- Endpoint test all endpoints in the API with HTTP requests using WebApplicationFactory.

To run tests one can use a test explorer in the code editor or run the command: `dotnet test`.
The same command is run as part of the deploy pipeline. Nothing will be deployed if any test fails.
