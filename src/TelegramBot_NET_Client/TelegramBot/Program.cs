using System.Security.AccessControl;
using System.Net;
using TelegramBot;
using RestSharp;
using TelegramBot.Client;

// var proxyAddress = "206.189.15.100:54330";
// var proxy = new WebProxy(proxyAddress);


var grpcClient = new GrpcClient();

Bot bot = new Bot(Configuration.BOT_TOKEN, Configuration.CHAT_ID, grpcClient);
await bot.Start();

Console.ReadLine();
bot.Stop();
