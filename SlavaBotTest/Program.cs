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
        private static string botChoice;
        private static int callbackResult; /*0 - Ничья, 1 - Клиент победил, 2 - Клиент проиграл*/
        private static Random random;
        private static object syncObj = new object();
        private static int GenerateRandomNumber(int from, int to)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random(); // Or exception...
                return random.Next(from, to);
            }
        }
        static void Main(string[] args)
        {
            var proxy = new HttpToSocks5Proxy("88.247.147.96", 1080);
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

            int rand = GenerateRandomNumber(1,4);
            switch (rand) {
                case 1:
                    botChoice = "Камень";
                    break;
                case 2:
                    botChoice = "Ножницы";
                    break;
                case 3:
                    botChoice = "Бумага";
                    break;
            }

            string result = e.CallbackQuery.Data;

            try
            {
                switch (rand) {
                    case 1: //Камень
                        switch (result) {
                            case "Камень":
                                callbackResult = 0;
                                break;
                            case "Ножницы":
                                callbackResult = 2;
                                break;
                            case "Бумага":
                                callbackResult = 1;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2: //Ножницы
                        switch (result)
                        {
                            case "Камень":
                                callbackResult = 1;
                                break;
                            case "Ножницы":
                                callbackResult = 0;
                                break;
                            case "Бумага":
                                callbackResult = 2;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 3: //Бумага
                        switch (result)
                        {
                            case "Камень":
                                callbackResult = 2;
                                break;
                            case "Ножницы":
                                callbackResult = 1;
                                break;
                            case "Бумага":
                                callbackResult = 0;
                                break;
                            default:
                                break;
                        }
                        break;

                    default:
                        break;
                }
            switch (callbackResult)
                {
                    case 0:
                        await botClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
$"И у меня {botChoice}. Ничья!");
                        break;
                    case 1:
                        await botClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
$"А у меня {botChoice}. Ты победил!");
                        break;
                    case 2:
                        await botClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
$"А у меня {botChoice}. Ты проиграл!");
                        break;
                    default:
                        break;
                                        }

               // await botClient.Ans(e.CallbackQuery.Id, $"Вы нажали {result}");
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

            //await botClient.SendTextMessageAsync(
            //    e.Message.Chat,
            //    $"You said {e.Message.Text}"
            //    ).ConfigureAwait(false);

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
