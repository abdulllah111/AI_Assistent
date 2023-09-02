using Grpc.Net.Client;
using TelegramBot;

namespace TelegramBot.Client;

public class GrpcClient
{
    private readonly AiService.AiServiceClient  _client;

    public GrpcClient()
    {
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var channel = GrpcChannel.ForAddress("https://localhost:7241", new GrpcChannelOptions { HttpHandler = httpHandler }); // Замените на адрес вашего gRPC-сервиса
        _client = new AiService.AiServiceClient(channel);
    }
    
    public async Task<string> SendMessage(string message)
    {
        var request = new PromtRequest { Message = message };
        var response = await _client.SendMessageAsync(request);
        return response.Response;
    }
}
