using Microsoft.AspNetCore.Mvc;
using Rabbit.Domain.Configuration;
using Rabbit.Domain.Models;
using Rabbit.WebApi.Services;

namespace Rabbit.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IRabbitMqService _rabbitMqService;
    
    public CustomersController(ICustomerService customerService, IRabbitMqService rabbitMqService)
    {
        _customerService = customerService;
        _rabbitMqService = rabbitMqService;
    }

    [HttpPost]
    public Domain.Models.Customer SignUp([FromBody] Rabbit.WebApi.Requests.CustomerRequest createCustomerRequest)
    {
        var customer = _customerService.SignUpCustomer(createCustomerRequest);

        _rabbitMqService.SendEvent(customer, RabbitQueues.Customers);

        return customer;
    }

    [HttpGet]
    public Domain.Models.Customer SignUpTest()
    {
        Rabbit.WebApi.Requests.CustomerRequest createCustomerRequest = new WebApi.Requests.CustomerRequest();
        createCustomerRequest.FirstName = "Mar";
        createCustomerRequest.LastName = "R";
        createCustomerRequest.EmailAddress = "r@mar.com";

        var customer = _customerService.SignUpCustomer(createCustomerRequest);

        _rabbitMqService.SendEvent(customer, RabbitQueues.Customers);

        return customer;
    }
}