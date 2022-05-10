//https://api.cloudamqp.com/
//loginpw dimex74@mail.ru

//https://habr.com/ru/post/488654/

//https://habr.com/ru/post/649915/
//docker run -d --hostname my-rabbit-host --name my-rabbit -e RABBITMQ_DEFAULT_USER=user -e RABBITMQ_DEFAULT_PASS=password rabbitmq:3-managemen
//docker run -it --rm --name rabbitmq -p 5672:5672 - p 15672:15672 rabbitmq: 3 - management
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
        //void SendMessage<T>(T model, ExchangeTypes type) where T : class;
        //Task<T> ReadMessage<T>(ExchangeTypes type) where T : class;
    }

    //public enum ExchangeTypes
    //{
    //    Direct,
    //    Fanout
    //}
    public class RabbitMqService : IRabbitMqService, IDisposable
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
                    // Включение автоматического восстановления
                    // соединения после сбоев сети 
                    AutomaticRecoveryEnabled = true             
                };

                
                this._connection = factory.CreateConnection();
            }

            return this._connection;
        }

        public void SendEvent<T>(T obj, string queueName)
        {
            using var channel = GetConnection().CreateModel();
            //https://habr.com/ru/post/490960/
            //создание очереди
            QueueDeclareOk queue = channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false,
                arguments: null);
            // queue — название очереди, которую мы хотим создать.Название должно быть уникальным и не может совпадать с системным именем очереди
            //durable — если true, то очередь будет сохранять свое состояние и восстанавливается после перезапуска сервера / брокера
            //exclusive — если true, то очередь будет разрешать подключаться только одному потребителю
            //autoDelete — если true, то очередь обретает способность автоматически удалять себя
            //arguments — необязательные аргументы.
            // Если создание очереди возможно, то сервер отправит клиенту синхронный RPC ответ Queue.DeclareOk.
            // Если создание очереди невозможно(произошел отказ по запросу Queue.Declare),
            // то канал закроется сервером при помощи команды Channel.Close и клиент получит исключение OperationInterruptedException,
            // которое будет содержать код ошибки и ее описание.
            if (queue != null) { }


            var json = JsonSerializer.Serialize(obj);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);

            _logger.LogInformation($"Successfully wrote event to Queue ({queueName})");
        }

        public void Dispose()
        {
            //if (_channel.IsOpen)
            //{
            //    _channel.Close();
            //}
            if (_connection != null)
            {
                if (_connection.IsOpen)
                {
                    _connection.Close();
                }
            }
          
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


//public async Task<T> ReadMessage<T>(ExchangeTypes type) where T : class
//{
//    T result = null;
//    var queueName = string.Empty;
//    switch (type)
//    {
//        case ExchangeTypes.Direct:
//            _model.BasicQos(0, 1, false);
//            break;
//        case ExchangeTypes.Fanout:
//            queueName = _model.QueueDeclare().QueueName;
//            _model.QueueBind(queueName, ExchangeName, string.Empty);
//            break;
//        default:
//            break;
//    }

//    var consumer = new AsyncEventingBasicConsumer(_model);
//    consumer.Received += async (sender, ea) =>
//    {
//        var body = ea.Body.ToArray();
//        var stringBody = Encoding.UTF8.GetString(body);
//        result = JsonConvert.DeserializeObject<T>(stringBody);
//        _model.BasicAck(ea.DeliveryTag, false);
//    };
//    switch (type)
//    {
//        case ExchangeTypes.Direct:
//            _model.BasicConsume(QueueName, true, consumer);
//            break;
//        case ExchangeTypes.Fanout:
//            _model.BasicConsume(queueName, true, consumer);
//            break;
//        default:
//            break;
//    }


//    return await Task.FromResult(result);
//}


//public void SendMessage<T>(T model, ExchangeTypes type) where T : class
//{
//    var jsonModel = JsonConvert.SerializeObject(model);
//    var byteMessage = Encoding.UTF8.GetBytes(jsonModel);
//    //exchange ==null => Direct type
//    /*
// * Exchange type
// *  1-Direct
// * 2-Fan Out   (pub/sub)
// * 3-Topic
// * 4-Header
// */
//    switch (type)
//    {

//        case ExchangeTypes.Direct:
//            {
//                var basicProperties = _model.CreateBasicProperties();
//                basicProperties.Persistent = true;

//                _model.BasicPublish(string.Empty, QueueName, false, basicProperties, byteMessage);
//                break;
//            }
//        case ExchangeTypes.Fanout:
//            _model.BasicPublish(ExchangeName, string.Empty, false, null, byteMessage);
//            break;
//        default:
//            break;
//    }


//}