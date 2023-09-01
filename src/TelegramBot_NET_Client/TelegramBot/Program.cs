using System.Security.AccessControl;
using System.Net;
using TelegramBot;
using TelegramBot.AI;
using TelegramBot.Models;
using RestSharp;
using OpenAI_API;
using OpenAI_API.Chat;

// var proxyAddress = "206.189.15.100:54330";
// var proxy = new WebProxy(proxyAddress);

Bot bot = new Bot(Configuration.BOT_TOKEN, Configuration.CHAT_ID);
await bot.Start();

Console.ReadLine();
bot.Stop();
