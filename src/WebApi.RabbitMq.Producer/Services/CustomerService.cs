using Microsoft.Extensions.Logging;
using Rabbit.Domain.Models;
using System;

namespace Rabbit.WebApi.Services
{
    public interface ICustomerService
    {
        Domain.Models.Customer SignUpCustomer(Rabbit.WebApi.Requests.CustomerRequest createCustomerRequest);
    }
    public class CustomerService : ICustomerService
    {
        private readonly ILogger<ICustomerService> _logger;

        public CustomerService(ILogger<ICustomerService> logger)
        {
            _logger = logger;
        }

        public Domain.Models.Customer SignUpCustomer(Rabbit.WebApi.Requests.CustomerRequest createCustomerRequest)
        {
            _logger.LogInformation($"Creating a customer called: {createCustomerRequest.FirstName} {createCustomerRequest.LastName}");

            return new Domain.Models.Customer
            {
                Id = Guid.NewGuid(),
                FirstName = createCustomerRequest.FirstName,
                LastName = createCustomerRequest.LastName,
                EmailAddress = createCustomerRequest.EmailAddress,
                CreatedAt = DateTime.Now
            };
        }
    }
}