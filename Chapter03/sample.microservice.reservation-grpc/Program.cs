using sample.microservice.reservation_grpc;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDaprClient();
var app = builder.Build();

app.UseRouting();

app.MapGrpcService<ReservationService>();
app.MapGet("/", async context =>
{
    await context.Response.WriteAsync
    (@"Communication with gRPC endpoints 
            must be made through a gRPC client. To 
            learn how to create a client, visit: 
            https://go.microsoft.com/fwlink/?linkid=2086909");
});
app.Run();
