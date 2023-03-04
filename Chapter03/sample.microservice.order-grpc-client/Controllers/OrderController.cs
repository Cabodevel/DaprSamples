using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using sample.microservice.order.Models;

namespace sample.microservice.order.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpPost("order")]
        public async Task<ActionResult<Guid>> SubmitOrder(Order order, [FromServices] DaprClient daprClient)
        {
            Console.WriteLine("Enter submit order");

            order.Id = Guid.NewGuid();
            foreach (var item in order.Items)
            {
                var data = new reservation_grpc.Generated.Item()
                {
                    SKU = item.ProductCode,
                    Quantity = item.Quantity
                };
                var result = await daprClient.
                 InvokeMethodGrpcAsync<reservation_grpc.Generated.Item,
                reservation_grpc.Generated.Item>("reservations","reserve", data);
                Console.WriteLine($"sku: {result.SKU} === new quantity: {result.Quantity}");
            }

            Console.WriteLine($"Submitted order {order.Id}");
            return order.Id;
        }
    }
}