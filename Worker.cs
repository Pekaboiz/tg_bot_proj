using TelegramBotBuilder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TelegramServiceHandler
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            BotController botController = new BotController(@"path", "token", "config.json");
            botController.TelegramConfig();

            try
            {
                botController.TelegramStart();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to start bot: {ex.Message}");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!botController.IsBotActive().Result)
                    {
                        _logger.LogWarning("Bot is down. Trying to restart...");
                        botController.TelegramStart();
                    }

                    await Task.Delay(600000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($": {ex.Message}");
                    botController.TelegramStart();
                }
            }
        }
    }
}
