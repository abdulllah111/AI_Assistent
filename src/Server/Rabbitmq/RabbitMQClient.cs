using Google.Protobuf;
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

  public async Task<string> SendMessageAsync(PromtRequest request)
  {
    var props = _channel.CreateBasicProperties();
    props.CorrelationId = Guid.NewGuid().ToString();
    
    // var body = Encoding.UTF8.GetBytes(message);
    var body = request.ToByteArray();


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
}