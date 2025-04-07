using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBuilder;
using TelegramServiceHandler.Logger;
using TelegramServiceHandler.TelegramBot;

namespace TelegramServiceHandler
{
    public class BotConfigurate : BotConfigurateBase
    {
        private static bool isReceivingStarted = false;
        private string tempPath;
        public string? logPath;
        public BotConfigurate(ITelegramBotClient _bot, string _tempPath) : base(_bot)
        {
            botClient = _bot;
            tempPath = _tempPath;
            receiverOptions = new ReceiverOptions();
            SetBotSettings();
        }


        public override async void SetBotSettings()
        {
            if (isReceivingStarted)
            {
                tgLog.Log($"{DateTime.Now} - StartReceiving already called. Skipping...");
                return;
            }

            using var cts = new CancellationTokenSource();
            receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // Allowed updates
            };

            botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), receiverOptions, cts.Token);

            isReceivingStarted = true;

            await Task.Delay(1000);
        }
       
        public override async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            await UpdateHandler.Instance.HandleUpdate(client, update, token, logPath);
        }

        public override async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            await Task.CompletedTask;
        }
    }
}
