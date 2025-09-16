using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
                   .WithMongoExpress()
                   .WithDataVolume();

var mongodb = mongo.AddDatabase("mongodb");

var apiService = builder.AddProject<Projects.Project_Xanthic_Xebu_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(mongodb)
    .WaitFor(mongodb);

builder.AddProject<Projects.Project_Xanthic_Xebu_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WaitFor(mongodb);

#pragma warning disable ASPIREHOSTINGPYTHON001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var pythonapp = builder.AddPythonApp("device1", "../Project-Xanthic-Xebu-Device", "main.py");
#pragma warning restore ASPIREHOSTINGPYTHON001

builder.Build().Run();
