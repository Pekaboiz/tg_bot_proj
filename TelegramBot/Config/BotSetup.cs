using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotBuilder;

namespace TelegramServiceHandler
{
    public class BotSetup : BotSetupBase
    {
        public BotSetup(string _path, TelgramBot _bot) : base(_path, _bot)
        {
            configPath = _path;
            bot = _bot;

            if (File.Exists(configPath))
            {
                LoadConfiguration();
            }
        }

        // Getting the configuration from the json file
        public override void LoadConfiguration()
        {
            string? json = File.ReadAllText(configPath);
            RootBot? root = JsonConvert.DeserializeObject<RootBot>(json);

            if (root != null)
            {
                bot = root.botConfig;
            }
        }
    }
}
