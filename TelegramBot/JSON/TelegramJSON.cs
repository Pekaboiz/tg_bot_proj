using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotBuilder;

namespace TelegramServiceHandler
{
    [Serializable]
    public class RootBot : RootBotBase
    {
        public new TelgramBot? botConfig;
    }

    public class TelgramBot : TelgramBotBase
    {
        public new string? Token { get; set; }
    }

    [Serializable]
    public class RootButtons
    {
        public List<ButtonsObj>? buttons { get; set; }
    }

    public class ButtonsObj
    {
        public string? actionText;
        public string? caption;
        public List<ButtonsAttrs>? buttons { get; set; }
    }
    
    public class ButtonsAttrs
    {
        public string? text { get; set; }
        public string? call { get; set; }
    }
}
