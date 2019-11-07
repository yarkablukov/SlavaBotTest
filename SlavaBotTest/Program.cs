using System;
using Telegram.Bot;
using MihaZupan;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using xNetStandard;
using System.Net.Http;
using Leaf;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using System.Net;

namespace SlavaBotTest
{
    class Program
    {
        private static ITelegramBotClient botClient;
        static void Main(string[] args)
        {
            var proxy = new HttpToSocks5Proxy("188.131.149.30", 1080);
            var proxyHttpClientHandler = new HttpClientHandler { Proxy = new WebProxy("85.99.228.87:42371"), UseProxy = true };
            var http_lient = new HttpClient(proxyHttpClientHandler);

            botClient = new TelegramBotClient("858308389:AAE22WRAHaWDXb2c9U4QsTQRT4zZZZ2Fr6U", proxy) { Timeout = TimeSpan.FromSeconds(300)};

            var mes = botClient.GetMeAsync().Result;

            Console.WriteLine("Bot id: {0}, bot name: {1}", mes.Id, mes.FirstName);

            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += BotOnCallbackQueryReceived;
            botClient.StartReceiving();


            Console.ReadKey();
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            if (e.CallbackQuery.Data == null)
                return;

            var rand = new Random().Next(1,3);
            /*
             * 1 - Камень
             * 2 - Ножницы
             * 3 - Бумага
             */

            string result = e.CallbackQuery.Data;

            //switch (e.CallbackQuery.Data)
            //{
            //    case ("Камень"):
            //        result = "Камень";
            //        break;
            //    case ("Ножницы"):
            //        result = "Ножницы";
            //        break;
            //    case ("Бумага"):
            //        result = "Бумага";
            //        break;
            //    default:
            //        break;
            //}

            try
            {
                await botClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы нажали {result}");
                Console.WriteLine("Сообщение доставлено");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Сообщение не прошло");
                Console.WriteLine(ex.Message);
            }
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e?.Message?.Text == null)
                return;
            Console.WriteLine("Received Text Message: {0} in chat: {1}", e.Message.Text, e.Message.Chat.Id);

            await botClient.SendTextMessageAsync(
                e.Message.Chat,
                $"You said {e.Message.Text}"
                ).ConfigureAwait(false);

            switch (e.Message.Text)
            {
                case "/start":
                    var inlineKeyboard = new InlineKeyboardMarkup(
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Камень"),
                            InlineKeyboardButton.WithCallbackData("Ножницы"),
                            InlineKeyboardButton.WithCallbackData("Бумага")
                        });
                    await botClient.SendTextMessageAsync(e.Message.Chat,
                        "Ваш выбор",
                        replyMarkup: inlineKeyboard
                      );
                    break;
                 default:
                    break;
            }
        }
    }
}
