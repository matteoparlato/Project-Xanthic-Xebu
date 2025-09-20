internal class Program
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var mongo = builder.AddMongoDB("mongo")
                           .WithMongoExpress()
                           .WithDataVolume()
                           .WithLifetime(ContainerLifetime.Persistent);

        var mongodb = mongo.AddDatabase("powermes");

        var rabbitmqUsername = builder.AddParameter("powermes-messagebroker-username", secret: true);
        var rabbitmqPassword = builder.AddParameter("powermes-messagebroker-password", secret: true);
        var rabbitmq = builder.AddRabbitMQ("powermes-messagebroker", rabbitmqUsername, rabbitmqPassword, 5672)
                              .WithDataVolume()
                              .WithManagementPlugin()
                              .WithLifetime(ContainerLifetime.Persistent);

        builder.AddProject<Projects.PowerMES_DeviceService>("powermes-deviceservice")
               .WithReference(mongodb)
               .WaitFor(mongo)
               .WaitFor(rabbitmq);

        builder.AddProject<Projects.PowerMES_Web>("powermes-web")
               //.WithReplicas(2)
               .WithReference(rabbitmq)
               .WaitFor(rabbitmq);

        builder.AddProject<Projects.PowerPMM_Web>("powerpmm-web")
               //.WithReplicas(2)
               .WithReference(rabbitmq)
               .WaitFor(rabbitmq);

        builder.AddProject<Projects.PowerSCADA_Web>("powerscada-web")
               //.WithReplicas(2)
               .WithReference(rabbitmq)
               .WaitFor(rabbitmq);

        builder.Build().Run();
    }
}