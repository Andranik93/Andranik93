using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotAboutInformation.Models
{
    class WeatherRoot
    {
        [JsonProperty("main")]
        public Weather Weather { get; set; }
    }
}
