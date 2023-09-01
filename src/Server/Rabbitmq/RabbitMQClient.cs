// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
// using System;
// using System.Text;

// namespace Server.Rabbitmq;

// public class RabbitMQClient
// {
//     private readonly ConnectionFactory factory;
//     private readonly IConnection connection;
//     private readonly IModel channel;
//     private readonly string queueName = "my_queue";
//     private EventingBasicConsumer consumer;

//     public RabbitMQClient()
//     {
//         consumer = new EventingBasicConsumer(channel);

//         consumer.Received += OnMessageReceived;

//         // channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

//         factory = new ConnectionFactory() { HostName = "localhost" , Port = 5672};
//         connection = factory.CreateConnection();
//         channel = connection.CreateModel();
//         channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
//     }

    
//     public void SendMessage(string message)
//     {
//         var body = Encoding.UTF8.GetBytes(message);
//         channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
//     }
//     private async Task OnMessageReceived(object model, BasicDeliverEventArgs ea)
//     {
        
//     }

//     public async Task<string> ReceiveMessage()
//     {
//         var tcs = new TaskCompletionSource<string>();

//         return await tcs.Task;
//     }
//     // public async Task<string> ReceiveMessage() 
//     // {
//     //     var tcs = new TaskCompletionSource<string>();

//     //     var consumer = new EventingBasicConsumer(channel);

//     //     consumer.Received += (model, ea) =>
//     //     {
//     //         var body = ea.Body.ToArray();
//     //         var response = Encoding.UTF8.GetString(body);
//     //         tcs.SetResult(response); 
//     //     };

//     //     channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

//     //     return await tcs.Task;
//     // }


//     // public async Task<string> ReceiveMessage()
//     // {
//     //     var consumer = new EventingBasicConsumer(channel);
//     //     var tcs = new TaskCompletionSource<string>();

//     //     consumer.Received += (model, ea) =>
//     //     {
//     //         var body = ea.Body.ToArray();
//     //         var response = Encoding.UTF8.GetString(body);
//     //         tcs.SetResult(response);
//     //     };

//     //     channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

//     //     return await tcs.Task;
//     // }
// }

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

namespace Server.Rabbitmq;

public class RabbitMQClient 
{
  private const string QUEUE_NAME = "my_queue";
  
  private readonly ConnectionFactory _factory;
  private readonly IConnection _connection;
  private readonly IModel _channel;

  public RabbitMQClient()
  {
    _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
    _connection = _factory.CreateConnection();
    _channel = _connection.CreateModel();

    _channel.QueueDeclare(queue: QUEUE_NAME,
                          durable: false,
                          exclusive: false, 
                          autoDelete: false,
                          arguments: null);
  }

  public void SendMessage(string message)
  {
    var body = Encoding.UTF8.GetBytes(message);

    _channel.BasicPublish(exchange: "",
                          routingKey: QUEUE_NAME,
                          basicProperties: null,
                          body: body);
  }

  public async Task<string> ReceiveMessage()
  {
    var consumer = new AsyncEventingBasicConsumer(_channel);

    var tcs = new TaskCompletionSource<string>();

    consumer.Received += async (model, ea) =>
    {
      var body = ea.Body.ToArray();
      var message = Encoding.UTF8.GetString(body);
      tcs.TrySetResult(message);
    };

    _channel.BasicConsume(queue: QUEUE_NAME,
                          autoAck: true,
                          consumer: consumer);

    // return await tcs.Task;
    return "dsadas";
  }
}