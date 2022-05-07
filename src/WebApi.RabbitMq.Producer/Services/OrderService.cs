using Microsoft.Extensions.Logging;
using Rabbit.Domain.Models;
using System;

namespace Rabbit.WebApi.Services
{
    public interface IOrderService
    {
        Domain.Models.Order CreateOrder(Rabbit.WebApi.Requests.OrderRequest createOrderRequest);
    }
    public class OrderService : IOrderService
    {
        private readonly ILogger<IOrderService> _logger;

        public OrderService(ILogger<IOrderService> logger)
        {
            _logger = logger;
        }

        public Domain.Models.Order CreateOrder(Rabbit.WebApi.Requests.OrderRequest createOrderRequest)
        {
            _logger.LogInformation($"New order request for: {createOrderRequest.ProductName} which costs ${createOrderRequest.ProductPrice}");

            return new Domain.Models.Order
            {
                Id = Guid.NewGuid(),
                ProductName = createOrderRequest.ProductName,
                ProductPrice = createOrderRequest.ProductPrice,
                CreatedAt = DateTime.Now
            };
        }
    }
}