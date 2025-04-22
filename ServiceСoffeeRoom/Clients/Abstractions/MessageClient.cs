using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ServiceСoffeeRoom.Clients.Abstractions
{
    public abstract class MessageClient
    {
        protected readonly ITelegramBotClient _client;
        protected MessageClient(ITelegramBotClient client)
        {
            _client = client;
        }
        readonly ConcurrentDictionary<long, Message> date = new();
        protected async Task<Message>  GetMessageById(long id, CancellationToken token) 
        {
            token.ThrowIfCancellationRequested();
            date.TryGetValue(id, out Message? oldMessage);
            if (oldMessage == null) throw new ArgumentNullException(nameof(oldMessage));
            return await Task.FromResult(oldMessage);
        }
        protected Message Add(Message message)
        {
            date.TryAdd(message.Chat.Id, message);
            return message;
        }

        protected async Task<long> Remove(long key, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            date.TryRemove(key, out Message? message);
            if (message is not null)
                await _client.DeleteMessageAsync(chatId: key, messageId: message.MessageId, token);
            return key;
        }

        protected async Task<Message> Update(Message message, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            date.TryGetValue(message.Chat.Id, out Message? oldMessage);
            if (oldMessage is not null)
            {
                await _client.DeleteMessageAsync(chatId: message.Chat.Id, messageId: oldMessage.MessageId, token);
                date.TryUpdate(message.Chat.Id, message, oldMessage);
            }
            else Add(message);
            return message;
        }
    }
}
