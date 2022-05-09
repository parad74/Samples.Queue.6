//https://api.cloudamqp.com/
//loginpw dimex74@mail.ru
using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rabbit.Domain.Configuration;
using RabbitMQ.Client;

namespace Rabbit.WebApi.Services
{
    public interface IRabbitMqService
    {
        void SendEvent<T>(T obj, string queueName);
        void SendMessage(object obj);
        void SendMessage(string message);
    }
    public class RabbitMqService : IRabbitMqService
    {
        private IConnection _connection;
        private readonly RabbitOptions _options;
        private readonly ILogger<IRabbitMqService> _logger;
        
        public RabbitMqService(IOptions<RabbitOptions> options, ILogger<IRabbitMqService> logger)
        {
            this._logger = logger;
            this._options = options.Value;
            GetConnection();
        }

        private IConnection GetConnection()
        {
            if (_connection is null)
            {
                var factory = new ConnectionFactory
                {
                    HostName = this._options.HostName,
                    UserName = this._options.UserName,
                    Password = this._options.Password ,
                    VirtualHost = this._options.UserName,
                };

                this._connection = factory.CreateConnection();
            }

            return this._connection;
        }

        public void SendEvent<T>(T obj, string queueName)
        {
            using var channel = GetConnection().CreateModel();
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false,
                arguments: null);

            var json = JsonSerializer.Serialize(obj);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);

            _logger.LogInformation($"Successfully wrote event to Queue ({queueName})");
        }

        //https://habr.com/ru/post/649915/
        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            // Не забудьте вынести значения "localhost" и "MyQueue"
            // в файл конфигурации
          //  var factory = new ConnectionFactory();
            //factory.Uri = new Uri("amqps://jfjcwtsi:B42QcuK7Chr_NoOU18aP1HdfrZxQrzqD@jackal.rmq.cloudamqp.com/jfjcwtsi");
            //factory.UserName = "jfjcwtsi";
            //factory.Password = "B42QcuK7Chr_NoOU18aP1HdfrZxQrzqD";
            //factory.VirtualHost = "jfjcwtsi";
            //factory.HostName = "jackal-01.rmq.cloudamqp.com";

            // using (var connection = factory.CreateConnection())
            // using (var channel = connection.CreateModel())
            using (var channel = GetConnection().CreateModel())
            {
                channel.QueueDeclare(queue: "MyQueue",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                               routingKey: "MyQueue",
                               basicProperties: null,
                               body: body);
            }
       }
    }
}