using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

namespace Server.Rabbitmq;

public class RabbitMQClient 
{

  private const string REQUEST_QUEUE = "request_queue";
  private const string RESPONSE_QUEUE = "response_queue";

  private readonly ConnectionFactory _factory;
  private readonly IConnection _connection;
  private readonly IModel _channel;
  private string _consumerTag;
  public RabbitMQClient()
  {
    _factory = new ConnectionFactory { HostName = "localhost" };
    _connection = _factory.CreateConnection();
    _channel = _connection.CreateModel();

    _channel.QueueDeclare(queue: REQUEST_QUEUE,
                          durable: false,
                          exclusive: false, 
                          autoDelete: false,
                          arguments: null);

    _channel.QueueDeclare(queue: RESPONSE_QUEUE,
                          durable: false,
                          exclusive: false, 
                          autoDelete: false,
                          arguments: null);
  }

  public async Task<string> SendMessageAsync(string message)
  {
    var props = _channel.CreateBasicProperties();
    props.CorrelationId = Guid.NewGuid().ToString();
    
    var body = Encoding.UTF8.GetBytes(message);

    _channel.BasicPublish(exchange: "",
                          routingKey: REQUEST_QUEUE,
                          basicProperties: props,
                          body: body);

    var consumer = new EventingBasicConsumer(_channel);
    
    var tcs = new TaskCompletionSource<string>();

    consumer.Received += (sender, ea) => 
    {
        if(ea.BasicProperties.CorrelationId == props.CorrelationId)
        {
          var response = Encoding.UTF8.GetString(ea.Body.ToArray());
          tcs.TrySetResult(response);
          _channel.BasicCancel(_consumerTag); 
        }
    };

    _consumerTag = _channel.BasicConsume(RESPONSE_QUEUE, true, consumer);
    
    return await tcs.Task;
  }
    
  // private const string REQUEST_QUEUE_NAME = "PromtRequests";
  // private const string RESPONSE_QUEUE_NAME = "PromtResponse";
  
  // private readonly ConnectionFactory _factory;
  // private readonly IConnection _connection;
  // private readonly IModel _channel;

  // public RabbitMQClient()
  // {
  //   _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
  //   _connection = _factory.CreateConnection();
  //   _channel = _connection.CreateModel();

  //   _channel.QueueDeclare(queue: REQUEST_QUEUE_NAME,
  //                         durable: false,
  //                         exclusive: false, 
  //                         autoDelete: false,
  //                         arguments: null);
  // }

  // public void SendMessage(string message)
  // {
  //   var body = Encoding.UTF8.GetBytes(message);
  //   var props = _channel.CreateBasicProperties();
  //   props.CorrelationId = Guid.NewGuid().ToString();
  //   _channel.BasicPublish(exchange: "",
  //                         routingKey: REQUEST_QUEUE_NAME,
  //                         basicProperties: props,
  //                         body: body);
  // }

  // public async Task<string> ReceiveMessage()
  // {
  //   _channel.QueueDeclare(queue: RESPONSE_QUEUE_NAME,
  //                         durable: false,
  //                         exclusive: false, 
  //                         autoDelete: false,
  //                         arguments: null);

  //   var consumer = new AsyncEventingBasicConsumer(_channel);

  //   var tcs = new TaskCompletionSource<string>();

  //   consumer.Received += async (model, ea) =>
  //   {
  //     var body = ea.Body.ToArray();
  //     var message = Encoding.UTF8.GetString(body);
  //     tcs.TrySetResult(message);
  //   };

  //   _channel.BasicConsume(queue: RESPONSE_QUEUE_NAME,
  //                         autoAck: true,
  //                         consumer: consumer);

  //   return await tcs.Task;
  //   // return "dsadas";
  
}