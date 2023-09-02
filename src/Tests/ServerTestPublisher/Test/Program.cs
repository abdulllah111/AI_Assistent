using Test;

var grpcClient = new GrpcClient();
string message = "Test1";
string response = await grpcClient.SendMessage(message);
Console.WriteLine("Response: " + response);