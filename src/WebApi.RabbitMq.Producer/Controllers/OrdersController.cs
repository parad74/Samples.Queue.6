using Microsoft.AspNetCore.Mvc;
using Rabbit.Domain.Configuration;
using Rabbit.Domain.Models;
using Rabbit.WebApi.Services;

namespace Rabbit.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IRabbitMqService _rabbitMqService;

    public OrdersController(IOrderService orderService, IRabbitMqService rabbitMqService)
    {
        _orderService = orderService;
        _rabbitMqService = rabbitMqService;
    }

    [HttpPost]
    public Domain.Models.Order Create([FromBody] Rabbit.WebApi.Requests.OrderRequest createOrderRequest)
    {
        var order = _orderService.CreateOrder(createOrderRequest);
        
        _rabbitMqService.SendEvent(order, RabbitQueues.Orders);

        return order;
    }

    [HttpGet]
    public Domain.Models.Order CreateTest()
    {
        Rabbit.WebApi.Requests.OrderRequest createOrderRequest = new WebApi.Requests.OrderRequest();
        createOrderRequest.ProductName = "ProductName1";
        createOrderRequest.ProductPrice = 45;
        
        var order = _orderService.CreateOrder(createOrderRequest);

        _rabbitMqService.SendEvent(order, RabbitQueues.Orders);

        return order;
    }
}