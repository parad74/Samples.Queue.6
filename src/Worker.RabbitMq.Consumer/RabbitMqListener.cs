using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Diagnostics;
using System;

//работает
public class RabbitMqListener : BackgroundService
{
	private IConnection _connection;
	private IModel _channel;

	public RabbitMqListener()
	{
		// вынести значения "localhost" и "MyQueue"
		// в файл конфигурации
		var factory = new ConnectionFactory();
		//factory.Uri = new Uri("amqps://jfjcwtsi:B42QcuK7Chr_NoOU18aP1HdfrZxQrzqD@jackal.rmq.cloudamqp.com/jfjcwtsi");
		//factory.UserName = "jfjcwtsi";
		//factory.Password = "B42QcuK7Chr_NoOU18aP1HdfrZxQrzqD";
		//factory.VirtualHost = "jfjcwtsi";
		//factory.HostName = "jackal-01.rmq.cloudamqp.com";
		factory.UserName = "guesti";
		factory.Password = "guest";
		factory.HostName = "localhost";
		// Включение автоматического восстановления
		// соединения после сбоев сети 
		factory.AutomaticRecoveryEnabled = true;

		this._connection = factory.CreateConnection();
		this._channel = _connection.CreateModel();
		this._channel.QueueDeclare(queue: "MyQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		stoppingToken.ThrowIfCancellationRequested();

		var consumer = new EventingBasicConsumer(this._channel);
		//var consumer = new AsyncEventingBasicConsumer(_model);
		consumer.Received += (ch, ea) =>
		{
			var body = ea.Body.ToArray();
			var content = Encoding.UTF8.GetString(body);
			
			// Каким-то образом обрабатываем полученное сообщение
			Debug.WriteLine($"Получено сообщение: {content}");
			//var result = JsonConvert.DeserializeObject<T>(content);
			//	string queueName = _channel.QueueDeclare().QueueName;
			this._channel.BasicAck(ea.DeliveryTag, false);
		};

		this._channel.BasicConsume("MyQueue", false, consumer);

		return Task.CompletedTask;
	}

	public override void Dispose()
	{
		if (_channel != null)
		{
			if (_channel.IsOpen)
			{
				_channel.Close();
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
