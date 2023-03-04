using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using sample.microservice.order.Models;

namespace sample.microservice.order.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpPost("order")]
        public async Task<ActionResult<Guid>> 
         SubmitOrder(Order order, [FromServices] DaprClient 
         daprClient)
        {
            Console.WriteLine("Enter submit order");
            
            order.Id = Guid.NewGuid();
            foreach (var item in order.Items)
            {
                var data = new { sku = item.ProductCode, 
                 quantity = item.Quantity };
                var result = await daprClient.InvokeMethodAsync<object, dynamic>
                (HttpMethod.Post, 
                 "reservation-service",
                  "reserve",
                   data);
                Console.WriteLine($"sku: {result.GetProperty("sku")} === new quantity: {result.GetProperty("quantity")}");
            }
            Console.WriteLine($"Submitted order {order.Id}");
            return order.Id;
        }
    }
}