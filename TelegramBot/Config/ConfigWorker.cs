using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotBuilder;
using System.IO;

namespace TelegramServiceHandler
{
    public class ConfigGenerator  // Better be singleton, but never mind
    {
        protected string token;
        protected string _tempFileName;
        public ConfigGenerator(string _path, string _token, string _name)
        {
            _tempFileName = _name;
            token = _token;

            GenerateConfig(_path);
        }

        // Create the configuration file for the bot
        protected void GenerateConfig(string path)
        {
            // write the configurations
            if (!File.Exists(Path.Combine(path, _tempFileName)))
            {
                MainTgConfig(path); 
            }
            else if (!File.Exists(Path.Combine(path, "btn_config.json")))
            {
                ButtonsConfig(path);
            }
        }

        protected void MainTgConfig(string _path)
        {
            string mainPath = Path.Combine(_path, _tempFileName);
            TelgramBot bot = new TelgramBot { Token = token }; // attributes of the bot
            RootBot rootBot = new RootBot { botConfig = bot }; // root of the bot

            JObject config = JObject.FromObject(rootBot);

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(mainPath, json); // write the configuration to the file
        }

        protected void ButtonsConfig(string _path)
        {
            // Making tamplate for the buttons
            // -- (actually u can load ur buttons from DB, would be easier, i'll do it by my self)
            string buttonPath = Path.Combine(_path, "btn_config.json");

            var attr1 = new ButtonsAttrs { text = "BtnText", call = "call1Query" };
            var attr2 = new ButtonsAttrs { text = "BtnText", call = "call1Query1" };

            List<ButtonsAttrs> attrsList = new List<ButtonsAttrs> { attr1, attr2 };

            var buttonsObj1 = new ButtonsObj { actionText = "/action", caption = "caption1", buttons = attrsList};
            var buttonsObj2 = new ButtonsObj { actionText = "/action2", caption = "caption1", buttons = attrsList };

            List<ButtonsObj> buttonsList = new List<ButtonsObj> { buttonsObj1, buttonsObj2 };

            RootButtons rootButtons = new RootButtons { buttons =  buttonsList } ;

            JObject config = JObject.FromObject(rootButtons);

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            //
            File.WriteAllText(buttonPath, json); // write the configuration to the buttons
        }

    }
}
