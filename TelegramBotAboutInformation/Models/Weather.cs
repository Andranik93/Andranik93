using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotAboutInformation.Models
{
    class Weather
    {
        public float temp { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
    }
}
