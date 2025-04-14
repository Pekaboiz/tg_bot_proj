using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramServiceHandler.ChatGPT;
using TelegramServiceHandler.Logger;

namespace TelegramServiceHandler.TelegramBot
{
    public class UpdateHandler
    {
        private ITelegramBotClient? client;
        private CancellationToken token;
        private Update? update;
        private string? tempPath;
        private ButtonsParser? parser;
        private ButtonsObj? buttonsObj;
        private OpenAiService? _ollamaService;

        private static readonly Lazy<UpdateHandler> _instance = new Lazy<UpdateHandler>(() => new UpdateHandler());

        public static UpdateHandler Instance => _instance.Value;

        public async Task SimulateTypingAsync(long chatId)
        {
            await client.SendChatAction(chatId, ChatAction.Typing);
        }
        
        public async Task HandleUpdate(ITelegramBotClient _client, Update _update, CancellationToken _token, string _tempPath)
        {
            client = _client;
            token = _token;
            update = _update;
            tempPath = _tempPath;
            _ollamaService = new OpenAiService("llama3");

            if (update.Type == UpdateType.CallbackQuery)
            {
               await CallbackQuery(update.CallbackQuery!);
            }
            else
            {
                await SimulateTypingAsync(update.Message.Chat.Id);
                await Messages();
            }
        }

        private async Task CallbackQuery(CallbackQuery callbackQuery)
        {
            string data = callbackQuery.Data; 
            long chatId = callbackQuery.Message.Chat.Id;

            await CallDictionaryHandler.Instance.HandleCallbackQuery(callbackQuery, client);

            await client.AnswerCallbackQuery(callbackQuery.Id);
        }

        private async Task Messages()
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;
            long chatId = message.Chat.Id;


            if (messageText.StartsWith("/"))
            {
                parser = new ButtonsParser(Path.Combine(tempPath, "btn_config.json"), messageText);
                if (parser.buttonsObj != null)
                {
                    buttonsObj = parser.buttonsObj;
                    var InlineKeybord = parser.inlineKeyboard;

                    await client.SendMessage(chatId, buttonsObj.caption!, replyMarkup: InlineKeybord, cancellationToken: token);
                }
                else
                {
                    await client.SendMessage(chatId, $"I dont have this command 😳", cancellationToken: token);
                }
            }
            else
            {
                string responseText = messageText switch
                {
                    "Hello" => "Hi, how are u?",
                    _ => await _ollamaService.GetAIResponseAsync(message.Text)
                };

                //var (userText, jsonText) = _ollamaService.SplitAiResponse(responseText);

                if (!string.IsNullOrEmpty(responseText))
                {
                    await client.SendMessage(chatId, responseText, cancellationToken: token);
                }

                /*if (!string.IsNullOrEmpty(jsonText))
                {
                    tgLog.Log($"JSON: {jsonText}");
                }*/
                
            }
        }
    }
}
