using Microsoft.Extensions.Hosting;

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

#pragma warning disable ASPIREHOSTINGPYTHON001#pragma warning disable ASPIREHOSTINGPYTHON001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.AddPythonApp("instrumented-python-app0", "../InstrumentedPythonProject", "app.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();
builder.AddPythonApp("instrumented-python-app1", "../InstrumentedPythonProject", "app.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();
builder.AddPythonApp("instrumented-python-app2", "../InstrumentedPythonProject", "app.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();
builder.AddPythonApp("instrumented-python-app3", "../InstrumentedPythonProject", "app.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();
builder.AddPythonApp("instrumented-python-app4", "../InstrumentedPythonProject", "app.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();
builder.AddPythonApp("instrumented-python-app5", "../InstrumentedPythonProject", "app.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();
builder.AddPythonApp("instrumented-python-app6", "../InstrumentedPythonProject", "app.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();
builder.AddPythonApp("instrumented-python-app7", "../InstrumentedPythonProject", "app.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();
builder.AddPythonApp("instrumented-python-app8", "../InstrumentedPythonProject", "app.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();
builder.AddPythonApp("instrumented-python-app9", "../InstrumentedPythonProject", "app.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();
#pragma warning restore ASPIREHOSTINGPYTHON001

builder.Build().Run();
