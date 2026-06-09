using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Delhivery.Application.DTOs.Order;
using Delhivery.Application.Interfaces;
using Delhivery.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Delhivery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;

    public OrdersController(IRepository<Order> orderRepository, IPaymentService paymentService, IConfiguration configuration)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _configuration = configuration;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userId = GetUserId();
        var orders = await _orderRepository.FindAsync(o => o.UserId == userId);
        return Ok(orders);
    }

    [Authorize]
    [HttpPost("initiate")]
    public async Task<IActionResult> InitiateOrder(OrderRequest request)
    {
        // Simple logic for cost calculation based on weight
        decimal amount = 150m; 

        var order = new Order
        {
            UserId = GetUserId(),
            PickupAddressId = request.PickupAddressId,
            DeliveryAddressId = request.DeliveryAddressId,
            PackagingType = request.PackagingType,
            WeightCategory = request.WeightCategory,
            ContentType = request.ContentType,
            ScheduledPickupDate = request.ScheduledPickupDate,
            Amount = amount
        };

        await _orderRepository.AddAsync(order);

        // Generate Razorpay Order
        var razorpayOrderId = await _paymentService.CreateOrderAsync(amount, order.Id.ToString());
        order.RazorpayOrderId = razorpayOrderId;
        
        await _orderRepository.UpdateAsync(order);

        return Ok(new { 
            OrderId = order.Id, 
            RazorpayOrderId = razorpayOrderId, 
            Amount = amount,
            Key = _configuration["Razorpay:Key"]
        });
    }

    [Authorize]
    [HttpPost("verify-payment")]
    public async Task<IActionResult> VerifyPayment(PaymentVerificationRequest request)
    {
        var isSignatureValid = _paymentService.VerifyPaymentSignature(request.OrderId, request.PaymentId, request.Signature);
        
        if (!isSignatureValid)
            return BadRequest("Invalid payment signature");

        var orders = await _orderRepository.FindAsync(o => o.RazorpayOrderId == request.OrderId);
        var order = orders.FirstOrDefault();
        
        if (order == null)
            return NotFound("Order not found");

        order.PaymentStatus = Delhivery.Domain.Enums.PaymentStatus.Success;
        order.Status = Delhivery.Domain.Enums.OrderStatus.Confirmed;
        await _orderRepository.UpdateAsync(order);

        return Ok(new { Message = "Payment verified successfully" });
    }
}
