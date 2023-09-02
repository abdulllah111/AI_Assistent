using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Server.Rabbitmq;

namespace Server.Services;

public class ServiceAI : AiService.AiServiceBase
{
    private readonly ILogger<ServiceAI> _logger;
    private readonly RabbitMQClient rabbitMQClient;

    public ServiceAI(ILogger<ServiceAI> logger, RabbitMQClient rabbitMQClient)
    {
        _logger = logger;
        this.rabbitMQClient = rabbitMQClient;
    }

    public override async Task<PromtResponse> SendMessage(PromtRequest request, ServerCallContext context)
    {
        // // Отправить сообщение в RabbitMQ
        // rabbitMQClient.SendMessage(request.Message);

        // // Дождаться ответа от Python-приложения
        // string response = await rabbitMQClient.ReceiveMessage();

        string response = await rabbitMQClient.SendMessageAsync(request.Message);

        return new PromtResponse { Response = response };
    }
}
