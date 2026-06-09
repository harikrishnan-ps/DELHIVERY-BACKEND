using Razorpay.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Delhivery.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                string key = _configuration["Razorpay:Key"];
                string secret = _configuration["Razorpay:Secret"];

                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret))
                {
                    return BadRequest(new { message = "Razorpay API keys are not configured properly." });
                }

                RazorpayClient client = new RazorpayClient(key, secret);

                Dictionary<string, object> options = new Dictionary<string, object>();
                // amount in the smallest currency unit (e.g. paise for INR)
                options.Add("amount", (int)(request.Amount * 100)); 
                options.Add("currency", "INR");
                options.Add("receipt", "receipt_" + System.Guid.NewGuid().ToString().Substring(0, 8));

                Order order = client.Order.Create(options);

                return Ok(new CreateOrderResponse
                {
                    OrderId = order["id"].ToString(),
                    Amount = request.Amount,
                    Currency = "INR",
                    Key = key
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class CreateOrderRequest
    {
        public decimal Amount { get; set; }
    }

    public class CreateOrderResponse
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Key { get; set; }
    }
}
