var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.VectorSearchDemos_Frontend>("vectorsearchdemos-frontend");

builder.Build().Run();
