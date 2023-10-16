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
using QuickType;
using System.Collections.Concurrent;

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
        private ConcurrentQueue<Message> _sendQueue = new ConcurrentQueue<Message>();

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
            await _botClient.DeleteWebhookAsync();
        }

        public void Stop(){
            _cts.Cancel();
        }

        private async Task HandleUpdateAsync(ITelegramBotClient _botClient, Update update, CancellationToken cancellationToken)
        {

            

            if (update.Message is not { } message)
                return ;

            if (message.Text is not { } messageText)
                return ;

            // Only process messages with the bot's username mentioned
            if (!messageText.Contains("@abdul_assistent_ai_bot "))
                return ;

            var isExist = await IsMessageExistsAsync(_botClient, message);
            if (!isExist)
                return ;

            var chatMember = await _botClient.GetChatMemberAsync(message.Chat.Id, message.From.Id);
            if (chatMember.Status == ChatMemberStatus.Kicked)
                return ;

               //Только в выбранном чате
            // if(message.Chat.Id != _chatId)
            //     return;

            _sendQueue.Enqueue(message);
             _ = SendMessageAsync(cancellationToken);
             
        }


        private async Task SendMessageAsync(CancellationToken cancellationToken)
        {
        // Извлекаем сообщение из очереди
        if (_sendQueue.TryDequeue(out var queuedMessage))
        {
            System.Console.WriteLine(queuedMessage.Text);
            // Отправляем запрос на сервер
            string response_json = await _grpcClient.SendMessage(queuedMessage.Text.Replace("@abdul_assistent_ai_bot ", ""), queuedMessage.From.Id.ToString());

            if(response_json == "NoN"){
                await _botClient.SendTextMessageAsync(
                    chatId: queuedMessage.Chat.Id,
                    text: $"No no no",
                    cancellationToken: cancellationToken,
                    replyToMessageId: queuedMessage.MessageId
                );
                return;
            }

            var response = Response.FromJson(response_json);
            // Отправляем ответ в Telegram
            var responseTasks = response.Select(async mess =>
            {
                Message sentMessage = await _botClient.SendTextMessageAsync(
                    chatId: queuedMessage.Chat.Id,
                    text: $"{mess.Title}\n\n {mess.Text}",
                    cancellationToken: cancellationToken,
                    replyToMessageId: queuedMessage.MessageId
                );

                return sentMessage;
            });
            await Task.WhenAll(responseTasks);
            
            
            // string response = await _grpcClient.SendMessage(queuedMessage.Text.Replace("@abdul_assistent_ai_bot ", ""), queuedMessage.From.Id.ToString());
            // await _botClient.SendTextMessageAsync(
            //         chatId: queuedMessage.Chat.Id,
            //         text: response,
            //         cancellationToken: cancellationToken,
            //         replyToMessageId: queuedMessage.MessageId
            //     );
        }
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