var builder = DistributedApplication.CreateBuilder(args);

var sqlServerPassword = builder.AddParameter("SqlServerPassword", secret: true);

var sqlServer = builder.AddSqlServer("sqlserver", sqlServerPassword, 1433)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var database = sqlServer.AddDatabase("filestreaming", databaseName: "FileStreaming");

var rabbitMq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.FileStreaming_Api>("api")
    .WithReference(database, connectionName: "ConnectionString")
    .WithReference(rabbitMq)
    .WaitFor(database)
    .WaitFor(rabbitMq);

builder.Build().Run();
