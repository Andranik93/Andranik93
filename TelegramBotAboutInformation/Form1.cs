using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotAboutInformation.Models;

namespace TelegramBotAboutInformation
{
    public partial class TelegramBot : Form
    {
        #region Private Vars
        readonly TelegramBotClient bot;
        HttpClient _clientForWeather;
        HttpClient _clientForRates;
        HtmlWeb _htmlWeBSite;
        string _baseUrlForWeather = string.Empty;
        string _baseUrlForRates = string.Empty;
        string _htmlUrl = string.Empty;
        string _news = string.Empty;
        WeatherRoot _weather;
        RateRoot _rate;
        #endregion

        #region Constructors
        [Obsolete]
        public TelegramBot()
        {
            InitializeComponent();
            _baseUrlForWeather = "https://api.openweathermap.org";
            _baseUrlForRates = "https://openexchangerates.org/api/";
            _htmlUrl = "https://news.am/eng/";


            _clientForWeather = new HttpClient();
            _clientForRates = new HttpClient();
            _htmlWeBSite = new HtmlWeb();

            _clientForWeather.BaseAddress = new Uri(_baseUrlForWeather);
            _clientForRates.BaseAddress = new Uri(_baseUrlForRates);
            _weather = new WeatherRoot();
            _rate = new RateRoot();

            bot = new TelegramBotClient("2036402865:AAEwjA9imFT-fiwoLoss-f5EtqznDdQ2UDY");
            InitiaizeBot();
        }

        #endregion

        #region Events
        [Obsolete]
        private void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var chatId = e.Message.Chat.Id;
            var text = e.Message.Text.ToString();

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if (text == "/start")
                {
                    bot.SendTextMessageAsync(chatId, "Choose prefered info", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: CreateButton());
                }
            }
        }

        [Obsolete]
        private async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var id = e.CallbackQuery.From.Id;
            var fullname = e.CallbackQuery.From.FirstName + " " + e.CallbackQuery.From.LastName;
            var text = e.CallbackQuery.Data.ToString();

            switch (text)
            {
                case "We":
                    try
                    {
                        await GetWeather();
                        await bot.EditMessageTextAsync(id, e.CallbackQuery.Message.MessageId, $"Weather in Yerevan is" +
                            $" {Math.Round(_weather.Weather.temp)}°C", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: null);
                        Thread.Sleep(1000);
                        await bot.SendTextMessageAsync(id, "Choose prefered info", replyMarkup: CreateButton());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case "Ex":
                    try
                    {
                        await GetRates();
                        await bot.EditMessageTextAsync(id, e.CallbackQuery.Message.MessageId, $"1 Dollar is {Math.Round(_rate.rates.AMD)} dram", replyMarkup: null);
                        Thread.Sleep(1000);
                        await bot.SendTextMessageAsync(id, "Choose prefered info", replyMarkup: CreateButton());
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                    break;
                case "Nu":
                    try
                    {
                        GetNews();
                        await bot.EditMessageTextAsync(id, e.CallbackQuery.Message.MessageId, $"Top " +
                            $"News\n{_news}", replyMarkup: null);
                        Thread.Sleep(1000);
                        await bot.SendTextMessageAsync(id, "Choose prefered info", replyMarkup: CreateButton());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Methods
        [Obsolete]
        private void InitiaizeBot()
        {
            bot.OnMessage += Bot_OnMessage;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
        }

        [Obsolete]
        private void StartButton_Click(object sender, EventArgs e)
        {
            bot.StartReceiving();
        }

        [Obsolete]
        private void StopButton_Click(object sender, EventArgs e)
        {
            bot.StopReceiving();
        }

        private IReplyMarkup CreateButton()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton { CallbackData = "We", Text = "Weather" },
                new InlineKeyboardButton { CallbackData = "Ex", Text = "Exchange" },
                new InlineKeyboardButton { CallbackData = "Nu", Text = "News" }
            };

            var menu = new List<InlineKeyboardButton[]>
            {
                new[] { buttons[0], buttons[1], buttons[2] }
            };
            var main = new InlineKeyboardMarkup(menu.ToArray());
            return main;
        }

        private async Task GetWeather()
        {
            string request = $"/data/2.5/weather?id=616052&appid=4d53583eef0dd1398503cf62015262e7";

            using (var response = await _clientForWeather.GetAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var jsonStr = await response.Content.ReadAsStringAsync();
                    _weather = JsonConvert.DeserializeObject<WeatherRoot>(jsonStr);
                    _weather.Weather.temp -= 273.15f;
                }
            }
        }

        private async Task GetRates()
        {
            string request = $"latest.json?app_id=e9701bd531794ce4a0c9e14e42ada88b";

            using (var response = await _clientForRates.GetAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var jsonStr = await response.Content.ReadAsStringAsync();
                    _rate = JsonConvert.DeserializeObject<RateRoot>(jsonStr);
                }
            }
        }

        private void GetNews()
        {
            var doc = _htmlWeBSite.Load(_htmlUrl);
            var nodes = doc.DocumentNode.SelectNodes("//div");
            foreach (var node in nodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    if (node.Attributes["class"]?.Value == "news-list short-top")
                    {
                        _news += node.InnerText;
                    }
                }
            }

        } 
        #endregion


    }
}
