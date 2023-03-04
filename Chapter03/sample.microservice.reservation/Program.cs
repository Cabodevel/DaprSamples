using System.Text.Json;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddDapr(builder => 
                builder.UseJsonSerializationOptions(
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = 
                         JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true,
                    }));;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapPost("reserve", Reserve);
});

async Task Reserve(HttpContext context)
{
    Console.WriteLine("Enter Reservation");
    
    var serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };
            
    // DaprClient could be used to interact with State store etc..
    var client = context.RequestServices.GetRequiredService<DaprClient>();

    var item = await JsonSerializer.DeserializeAsync<Item>(context.Request.Body,
                                                            serializerOptions);
    
    // your business logic should be here

    /* a specific type is used in sample.microservice.reservation and not
    reused the class in sample.microservice.order with the same signature: 
    this is just to not introduce DTO and to suggest that it might be a good idea
    having each service separating the type for persisting store */
    Item storedItem;    
    // from store? state?
    storedItem = new Item();
    storedItem.SKU = item.SKU;
    storedItem.Quantity -= item.Quantity;

    Console.WriteLine($"Reservation of {storedItem.SKU} is now {storedItem.Quantity}");

    context.Response.ContentType = "application/json";
    await JsonSerializer.SerializeAsync(context.Response.Body, storedItem, serializerOptions);
}

app.Run();
