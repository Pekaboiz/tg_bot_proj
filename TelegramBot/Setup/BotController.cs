using Telegram.Bot;
using TelegramBotBuilder;

namespace TelegramServiceHandler
{
    internal class BotController : BotControllerBase
    {
        protected BotConfigurate? botConf;
        protected string _path { get; set; }

        public BotController(string path, string token, string nameJSON) : base(path, token, nameJSON)
        {
            _tempPath = Path.Combine(path, nameJSON);
            _path = path;
            _token = token;
            _nameJSON = nameJSON;
        }

        public override void TelegramConfig()
        {
            if (_path.Length > 0 && _token.Length > 0)
            {
                ConfigGenerator config = new ConfigGenerator(_path, _token, _nameJSON);
            }
        }

        public override void TelegramStart()
        {
            BotConnect();
            if (botClient != null) { 
                botConf = new BotConfigurate(botClient, _tempPath);
                botConf.logPath = _path;
            }
        }

        public async override Task<bool> IsBotActive()
        {
            if (botClient != null)
            {
                if (await botClient.GetMe() == null)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        // Connect Telegram Bot
        public override void BotConnect()
        {
            if (_tempPath != null)
            {
                TelgramBot bot = new TelgramBot();
                BotSetup telegram = new BotSetup(_tempPath, bot);
                TelgramBot? _bot = (TelgramBot?)telegram.bot;
                if (_bot != null && _bot.Token != null)
                {
                    botClient = new TelegramBotClient(_bot.Token);
                }
            }
        }

    }
}
