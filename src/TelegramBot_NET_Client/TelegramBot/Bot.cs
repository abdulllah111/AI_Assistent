using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.AI;
using RestSharp;


namespace TelegramBot
{

    public class Bot
    {
        private readonly long _chatId;
        private ITelegramBotClient _botClient;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        private ReceiverOptions receiverOptions = new ()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        public Bot(string bot_token, long chat_id, WebProxy proxy = null)
        {
            _botClient = new TelegramBotClient(bot_token);
            _chatId = chat_id;
        }

        public async Task Start(){
            // bot conf
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: _cts.Token
            );
            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");

        }

        public void Stop(){
            _cts.Cancel();
        }

        private async Task HandleUpdateAsync(ITelegramBotClient _botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;
            
            Console.WriteLine($"Received a '{messageText}' message in chat {message.Chat.Id }.");


            //Только в выбранном чате
            if(message.Chat.Id != _chatId)
                return;


            // Echo received message text
            Message sentMessage = await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: messageText,
                cancellationToken: cancellationToken,
                replyToMessageId: message.MessageId
            );

            await Task.Delay(10);


        }

        private Task HandlePollingErrorAsync(ITelegramBotClient _botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}