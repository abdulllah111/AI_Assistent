using Grpc.Net.Client;
using TelegramBot;

namespace TelegramBot.Client;

public class GrpcClient
{
    private readonly TelegramClientService.TelegramClientServiceClient  _client;

    public GrpcClient()
    {
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var channel = GrpcChannel.ForAddress("http://localhost:5023", new GrpcChannelOptions { HttpHandler = httpHandler }); // Замените на адрес вашего gRPC-сервиса
        _client = new TelegramClientService.TelegramClientServiceClient(channel);
    }
    
    public async Task<string> SendMessage(string message, string userId)
    {
        var request = new PromtRequest { Message = message, Userid = userId };
        var response = await _client.SendMessageAsync(request);
        return response.Response;
    }
}
