using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rabbit.Domain.Configuration;
using Rabbit.Domain.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker.RabbitMq.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
       // private readonly IOptions<RabbitOptions> _options;
        private IConnection _connection;

        private IModel _ordersChannel;
        private IModel _customersChannel;

        private readonly RabbitOptions _options;

        public Worker(ILogger<Worker> logger, IOptions<RabbitOptions> options)
        {
            this._logger = logger;
            this._options = options.Value; 

            InitializeConnection();
        }

        private void InitializeConnection()
        {
            var factory = new ConnectionFactory
            {
                 HostName = this._options.HostName,
                UserName = this._options.UserName,
                Password = this._options.Password,
                VirtualHost = this._options.UserName,
                // Включение автоматического восстановления
                // соединения после сбоев сети 
                AutomaticRecoveryEnabled = true
            };

            _connection = factory.CreateConnection();

            this._customersChannel = this._connection.CreateModel();
            this._customersChannel.QueueDeclare(queue: RabbitQueues.Customers, durable: false, exclusive: false, autoDelete: false,
                arguments: null);

            this._ordersChannel = _connection.CreateModel();
            this._ordersChannel.QueueDeclare(RabbitQueues.Orders, durable: false, exclusive: false, autoDelete: false,
                arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var customerConsumer = new EventingBasicConsumer(this._customersChannel);
            customerConsumer.Received += CustomerMessageReceived;
            this._customersChannel.BasicConsume(queue: RabbitQueues.Customers, autoAck: false, consumer: customerConsumer);

            var orderConsumer = new EventingBasicConsumer(_ordersChannel);
            orderConsumer.Received += OrderMessageReceived;
            this._ordersChannel.BasicConsume(queue: RabbitQueues.Orders, autoAck: false, consumer: orderConsumer);

            return Task.CompletedTask;
        }

        private void CustomerMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            var customer = DeserializeMessage<Customer>(e.Body.ToArray());

            Debug.WriteLine($"(Consumer) Received customer called: {customer.FirstName} {customer.LastName}");

            this._customersChannel!.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
        }

        private void OrderMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            var order = DeserializeMessage<Order>(e.Body.ToArray());

            Debug.WriteLine($"(Consumer) Received order for: {order.ProductName} at price: {order.ProductPrice}");

            this._ordersChannel!.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
        }

        private T DeserializeMessage<T>(byte[] bytes)
        {
            var asString = Encoding.UTF8.GetString(bytes);

            return JsonSerializer.Deserialize<T>(asString)!;
        }

        public override void Dispose()
        {
             if (_customersChannel != null)
            {
                if (_customersChannel.IsOpen)
                {
                    _customersChannel.Close();
                }
            }

            if (_ordersChannel != null)
            {
                if (_ordersChannel.IsOpen)
                {
                    _ordersChannel.Close();
                }
            }
            if (_connection != null)
            {
                if (_connection.IsOpen)
                {
                    _connection.Close();
                }
            }
            base.Dispose();
        }
    }
}