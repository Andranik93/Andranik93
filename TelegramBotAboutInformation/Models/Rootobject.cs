using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotAboutInformation.Models
{
    class RateRoot
    {
        public string disclaimer { get; set; }
        public string license { get; set; }
        public int timestamp { get; set; }
        public string _base { get; set; }
        public Rates rates { get; set; }
    }
}
