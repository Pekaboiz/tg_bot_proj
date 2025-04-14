using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

#pragma warning disable CS8603 // can be null that's okay
namespace TelegramServiceHandler
{
    public class ButtonsParser
    {
        public RootButtons? botParse;
        public ButtonsObj? buttonsObj;
        public InlineKeyboardMarkup? inlineKeyboard;

        private static DateTime _lastReadTime;
        public static RootButtons? _cachedButtons;
        private static string _lastConfigPath = "";

        public ButtonsParser(string _configPath, string _command)
        {
            ShowButtons(_configPath, _command);
        }

        private void ShowButtons(string configPath, string _command)
        {
            if (string.IsNullOrWhiteSpace(configPath)) return;

            if (_cachedButtons == null ||
                _lastConfigPath != configPath ||
                File.GetLastWriteTime(configPath) > _lastReadTime)
            {
                string json = File.ReadAllText(configPath);
                _cachedButtons = JsonConvert.DeserializeObject<RootButtons>(json);
                _lastReadTime = DateTime.Now;
                _lastConfigPath = configPath;
            }

            botParse = _cachedButtons;

            if (botParse?.buttons == null) return;
            
            foreach (var button in botParse.buttons)
            {
                if (button != null && (button.actionText == _command) )
                {
                    buttonsObj = button;
                    inlineKeyboard = new InlineKeyboardMarkup(
                        button.buttons!
                        .Select((btn, index) => new { btn, index })
                            .GroupBy(x => x.index / 2)
                            .Select(group => group
                            .Select(x => InlineKeyboardButton.WithCallbackData($"   {x.btn.text!}   ", x.btn.call!))
                            .ToArray())
                            .ToArray()
                    );
                }
            }
        }
    }
}
