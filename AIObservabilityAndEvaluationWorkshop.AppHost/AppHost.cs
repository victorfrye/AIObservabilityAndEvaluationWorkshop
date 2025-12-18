using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<AIObservabilityAndEvaluationWorkshop_ConsoleRunner>("console-app")
    .WithExplicitStart();

builder.Build().Run();
