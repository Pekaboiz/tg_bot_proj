using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramServiceHandler
{
    public class CallDictionaryHandler
    {
        private static readonly Lazy<CallDictionaryHandler> _instance = new Lazy<CallDictionaryHandler>(() => new CallDictionaryHandler());

        private readonly Dictionary<string, Func<CallbackQuery, Task> > _callActions;

        public static CallDictionaryHandler Instance => _instance.Value;

        private ITelegramBotClient client;

        private CallDictionaryHandler() 
        {

            _callActions = new Dictionary<string, Func<CallbackQuery, Task>> 
            { 
                {"call1Query", HandleCallQuery} 
            };
        
        }

        public async Task HandleCallbackQuery(CallbackQuery callbackQuery, ITelegramBotClient _client)
        {
            string data = callbackQuery.Data;
            client = _client;

            if (_callActions.ContainsKey(data))
            {
                // Если обработчик для этой кнопки найден, вызываем его
                await _callActions[data](callbackQuery);
            }
            else
            {
                // Если обработчик не найден, выводим сообщение
                await DefaultCallbackQueryHandler(callbackQuery);
            }
        }

        private async Task HandleCallQuery(CallbackQuery query)
        {
            long chatId = query.Message.Chat.Id;
            string response = "You pressed the first button!";

            await client.SendMessage(chatId, response);
        }

        // Метод по умолчанию для обработки незнакомых callback_data
        private async Task DefaultCallbackQueryHandler(CallbackQuery callbackQuery)
        {
            long chatId = callbackQuery.Message.Chat.Id;
            string response = "Sorry, I don't know what to do with that!";
            await client.SendMessage(chatId, response);
        }
    }
}
