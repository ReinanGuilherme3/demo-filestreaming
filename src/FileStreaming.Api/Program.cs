using FileStreaming.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSettings()
    .AddApplication()
    .AddInfrastructure()
    .AddSwagger();

builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MigrateDatabase();

app.Run();
