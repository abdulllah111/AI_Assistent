using Test;

var grpcClient = new GrpcClient();
string message = "Testtt";
string response = await grpcClient.SendMessage(message);
Console.WriteLine("Response: " + response);