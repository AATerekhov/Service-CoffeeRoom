using ServiceCoffeeRoom.Services.Applications.DtoModel.Person;
using ServiceCoffeeRoom.Services.Applications.DtoModel.Room;
using ServiceСoffeeRoom.Applications.Abstractions;
using ServiceСoffeeRoom.Applications.DtoModel.Person;
using ServiceСoffeeRoom.Clients.Abstractions;
using ServiceСoffeeRoom.Extentions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ServiceСoffeeRoom.Clients
{
    public class DialogueClient : MessageClient
    {
        private readonly IPersonService _personService;
        private readonly IRoomService _roomService;
        private readonly RoomClient _roomClient;
        private readonly CoffeMachineClient _coffeMachineClient;
        public DialogueClient(
        ITelegramBotClient client,
        IPersonService personService,
        IRoomService roomService,
        RoomClient roomClient,
        CoffeMachineClient coffeMachineClient) : base(client)
        {
            _personService = personService;
            _roomService = roomService;
            _roomClient = roomClient;
           _coffeMachineClient = coffeMachineClient;

        }
        public async Task<Message> ProcessingMessage(Message message, CancellationToken token)
        {        
            //Идентификация

            if (message.From == null) throw new ArgumentNullException(nameof(message.From));
            
            var thisPerson = await _personService.GetPersonByIdAsync(message.From.Id, token);
            if (thisPerson is null)
            {
                var porsonInfo = new CreatePersonDto()
                {
                    Id = message.From.Id,
                    Name = message.From.FirstName + " " + message.From.LastName,
                    TelegramAccaunt = string.IsNullOrEmpty(message.From.Username) ? string.Empty : message.From.Username,
                };
                thisPerson = await _personService.CreatePersonAsync(porsonInfo, token);
            }
            else if (string.IsNullOrEmpty(thisPerson.Name))
            {
                var porsonInfo = new UpdatePersonDto()
                {
                    Id = message.From.Id,
                    Name = message.From.FirstName + " " + message.From.LastName,
                    TelegramAccaunt = string.IsNullOrEmpty(message.From.Username) ? string.Empty : message.From.Username
                };
                if(await _personService.UpdatePersonAsync(porsonInfo, token))
                    thisPerson = await _personService.GetPersonByIdAsync(message.From.Id, token);
            }
            var chatId = message.Chat.Id;

            //Операции отмены диалогов
            _ = message.Text switch
            {
                "Выйти из админки" => await Remove(await _roomClient.RemoveRoomDialog(chatId, token), token),
                "Выйти из комнаты" => await Remove(await _coffeMachineClient.RemoveDialog(chatId, token), token),
                "Отмена" => await Remove(await DeleteServiceMessage(message, token), token),
                _ => chatId
            };

            if (thisPerson is null) throw new ArgumentException(nameof(thisPerson));
            //Валидация запроса.
            var resultValid = message.Text switch
            {
                "/start" => await _client.SendTextMessageAsync(await DeleteServiceMessage(message, token), TextMessages.Greeting(thisPerson.Name), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, cancellationToken: token),
                _ => message
            };

            if (message.Text == "Отмена")
                return resultValid;

            //Проверка на ожидание ввода данных.
            var oldMessage =await GetMessageById(chatId, token);
            if (oldMessage is not null && !string.IsNullOrEmpty(oldMessage.Caption))
            {
                var result = oldMessage.Caption switch
                {
                    "/roomAddUser" => await SendReplyKeyboard(await _roomClient.AddUserInRoom(await DeleteServiceMessage(message, token), message.UserShared!.UserId, token), TextMessages.Instruction("Кофейной комнаты"), Keyboards.ForRoomAdmin, token),
                    "/roomAddBeans" => await SendReplyKeyboard(await _roomClient.AddBeansInRoom(await DeleteServiceMessage(message, token), message, token), TextMessages.Instruction("Кофейной комнаты"), Keyboards.ForRoomAdmin, token),
                    "/roomEditServiceCoins" => await SendReplyKeyboard(await _roomClient.EditServiceCoinsInRoom(await DeleteServiceMessage(message, token), message, token), TextMessages.Instruction("Кофейной комнаты"), Keyboards.ForRoomAdmin, token),
                    "/roomEditServiceInterval" => await SendReplyKeyboard(await _roomClient.EditServiceIntervalInRoom(await DeleteServiceMessage(message,token), message, token), TextMessages.Instruction("Кофейной комнаты"), Keyboards.ForRoomAdmin, token),
                    "/roomEditNameRoom" => await SendReplyKeyboard(await _roomClient.EditServiceNameInRoom(await DeleteServiceMessage(message, token), message, token), TextMessages.Instruction("Кофейной комнаты"), Keyboards.ForRoomAdmin, token),
                    //"/roomDeleteUser" =>,
                    //"/roomEditAdmin" =>,
                    "/userUse" => await SendReplyKeyboard(await _coffeMachineClient.UseCoffeeMachine(await DeleteServiceMessage(message, token), message, thisPerson!, token), TextMessages.Use(), Keyboards.ForCoffeemachineUse, token),
                    "/userAddBalance" => await SendReplyKeyboard(await _coffeMachineClient.AddCash(await DeleteServiceMessage(message, token), message, thisPerson!, token), TextMessages.Use(), Keyboards.ForCoffeemachineUse, token),
                    "/userAddService" => await SendReplyKeyboard(await _coffeMachineClient.AddServiceProcedureAsync(await DeleteServiceMessage(message, token), thisPerson!, token), TextMessages.Use(), Keyboards.ForCoffeemachineUse, token),
                    _ => message
                };
                if (result.Equals(message) is false)
                    return result;
            }                

            //Получаем комнату.
            var room = await _roomService.GetRoom();
            if (room is not null)
            {
                if (message.Text is "Воспользоваться" || message.Text is "/coffee")
                {
                    _ = await _roomClient.RemoveRoomDialog(chatId, token);
                    return await SendReplyKeyboard(await _coffeMachineClient.StartCoffeeMachine(await DeleteTheOfficialQuestionAndAnswer(message, token), thisPerson!, token), TextMessages.Use(), Keyboards.ForCoffeemachineUse, token); 
                }
                if (thisPerson.IsAdmin)
                    return await AdminBehavior(message, thisPerson, token);
                else
                    return await UserBehavior(message, thisPerson, room, token);
            }
            else
            {
                //Если комнаты нет, то ее можно только создать
                if (message.Text is "Создать комнату")
                    return await SendReplyKeyboard(await _roomClient.StartRoom(await DeleteServiceMessage(message, token), thisPerson, token), TextMessages.Instruction("Комнаты"), Keyboards.ForRoomAdmin, token);
                else
                    return await SendReplyKeyboard(await DeleteServiceMessage(message, token), TextMessages.CreateRoom(), Keyboards.ForCreateRoom, token);
            }
        }

        private async Task<Message> UserBehavior(Message message, PersonDto thisPerson, RoomDto room, CancellationToken token)
        {
            if (thisPerson.IsUser)
                return await SendReplyKeyboard(await DeleteServiceMessage(message, token), TextMessages.UserRights(thisPerson.Name), Keyboards.ForUserRightsRoom, token);
            else
                return await SendReplyKeyboard(await DeleteServiceMessage(message, token), TextMessages.GetHolloyAndGoToAdmin(room.Name, room.Admin.TelegramAccaunt), Keyboards.ForCancel, token);
        }

        private async Task<Message> AdminBehavior(Message message, PersonDto thisPerson, CancellationToken token)
        {
            if (message.Text is "Администрировать" || message.Text is "/admin")
            {
                _ = await _coffeMachineClient.RemoveDialog(message.Chat.Id, token);
                return await SendReplyKeyboard(await _roomClient.StartRoom(await DeleteTheOfficialQuestionAndAnswer(message, token), thisPerson, token), TextMessages.Instruction("Кофейной комнаты"), Keyboards.ForRoomAdmin, token);
            }

            if (thisPerson.IsUser)
                return await SendReplyKeyboard(await DeleteServiceMessage(message, token), TextMessages.FullRights(), Keyboards.ForFullRightsRoom, token);
            else
                return await SendReplyKeyboard(await DeleteServiceMessage(message, token), TextMessages.AdminRights(thisPerson.Name), Keyboards.ForAdminRightsRoom, token);
        }

        private async Task<long> DeleteTheOfficialQuestionAndAnswer(Message message, CancellationToken token)
        {
            _ = await Remove(message.Chat.Id, token);
            return await DeleteServiceMessage(message, token);
        }

        private async Task<long> DeleteServiceMessage(Message message, CancellationToken token)
        {
            var result = message.Chat.Id;
            await _client.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId, cancellationToken: token);
            return result;
        }

        public async Task<Message> SendReplyKeyboard(long chatId, string text, Func<ReplyKeyboardMarkup> keyboard, CancellationToken token = default)
        {
            var message = await _client.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: keyboard(),
                cancellationToken: token);
            await Update(message, token);
            return message;
        }

        public async Task<Message> SendReplyKeyboardExpectedResponse(long chatId,string caption ,string text, Func<ReplyKeyboardMarkup> keyboard, CancellationToken token = default)
        {
            var message = await _client.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                parseMode:Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: keyboard(),
                cancellationToken: token);
            message.Caption = caption;
            await Update(message, token);
            return message;
        }
    }
}
