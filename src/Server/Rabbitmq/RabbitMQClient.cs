using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Server.Rabbitmq;

public class RabbitMQClient
{
    private readonly ConnectionFactory factory;
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName = "my_queue";

    public RabbitMQClient()
    {
        factory = new ConnectionFactory() { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }

    public async Task<string> ReceiveMessage()
    {
        var consumer = new EventingBasicConsumer(channel);
        var tcs = new TaskCompletionSource<string>();

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            tcs.SetResult(response);
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        return await tcs.Task;
    }
}
