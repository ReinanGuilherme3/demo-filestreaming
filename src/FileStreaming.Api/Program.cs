using FileStreaming.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApi()
    .AddSettings()
    .AddApplication()
    .AddInfrastructure()
    .AddSwagger();


var app = builder.Build();

app.UseApi()
   .UseSwagger()
   .MigrateDatabase()
   .Run();
