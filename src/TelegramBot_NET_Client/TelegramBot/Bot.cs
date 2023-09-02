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
using RestSharp;
using TelegramBot.Client;


namespace TelegramBot
{

    public class Bot
    {
        private readonly long _chatId;
        private ITelegramBotClient _botClient;
        private GrpcClient _grpcClient;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private ReceiverOptions receiverOptions = new ()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        public Bot(string bot_token, long chat_id, GrpcClient grpcClient,WebProxy proxy = null)
        {
            _botClient = new TelegramBotClient(bot_token);
            _chatId = chat_id;
            _grpcClient = grpcClient;
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
            

            var isExist = await IsMessageExistsAsync(_botClient, message);
            if(!isExist)
                return;

            var chatMember = await _botClient.GetChatMemberAsync(message.Chat.Id, message.From.Id);
            if(chatMember.Status == ChatMemberStatus.Kicked)
                return;

            string response = await _grpcClient.SendMessage(messageText);
            Console.WriteLine("Response: " + response);
            

            Message sentMessage = await _botClient.SendTextMessageAsync(
            chatId: _chatId,
            text: response,
            cancellationToken: cancellationToken
            // replyToMessageId: message.MessageId
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

        private async Task<bool> IsMessageExistsAsync(ITelegramBotClient botClient, Message message)
        {
            try
            {
                // Получаем информацию о пользователе, отправившем сообщение
                await botClient.GetChatMemberAsync(message.Chat.Id, message.From.Id);
                return true; // Сообщение существует
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 400 && ex.Message.Contains("ChatMemberNotFoundException"))
            {
                return false; // Сообщение не существует
            }
        }

    }
}