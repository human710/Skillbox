using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace HW_9_TelegramBot
{
    public class TelegrammClass
    {
        private static string token = "5292001260:AAECjsY3A-tKHtedpt76BlR68uprUnicDr8";
        private static TelegramBotClient bot = new(token);

        public TelegrammClass()
        {
            Telegramm();
        }

        public static void Telegramm()
        {
            var me = bot.GetMeAsync().Result;
            Console.WriteLine(me.Username);
            bot.OnMessage += OnMessageHandler;
            bot.StartReceiving();


            Console.ReadLine();
            bot.StopReceiving();
        }

        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var chatId = e.Message.Chat.Id;
            KeyboardMarkups();
            Console.WriteLine($"{e.Message.Chat.FirstName} {e.Message.Chat.LastName}: {e.Message.Text}");

            if (e.Message.Text != null)
                switch (e.Message.Text.ToLower())
                {
                    case "/start":
                        bot.SendTextMessageAsync(chatId,
                            "Приветствую! Данный чат бот позволяет принимать, хранить и скачивать файлы. " +
                            "Функциональные возможности будут расширяться и дорабатываться.\nНажмите кнопку 'Список файлов' " +
                            "чтобы посмореть доступные для загрузки файлы");
                        break;
                    case "hi":
                        bot.SendTextMessageAsync(chatId, "Привет, как дела?");
                        break;
                    case "список файлов":
                        int count = 0;
                        //string[] files = Directory.GetFiles("DownloadFiles");
                        //foreach (string s in files)
                        //{
                        //    var fInfo = new FileInfo(s);
                        //    //Console.WriteLine($"{++count}. {fInfo.Name}");
                        //    //await bot.SendTextMessageAsync(chatId, $"{fInfo.Name}");
                        //    CallbackMarkup(chatId);
                        //}

                        CallbackMarkup(chatId);
                        //bot.SendTextMessageAsync(chatId, FilesList());
                        break;
                    case "":
                        break;

                    
                    default:
                        await bot.SendTextMessageAsync(chatId, "Непонятная задача", replyMarkup: KeyboardMarkups());
                        break;
                }

            if (e.Message.Type == MessageType.Document)
            {
                if (!Directory.Exists("DownloadFiles"))
                    Directory.CreateDirectory("DownloadFiles");
                DownloadFile(e.Message.Document.FileId, @"DownloadFiles\" + e.Message.Document.FileName);
            }

            if (e.Message.Type == MessageType.Photo)
            {
                var file = await bot.GetFileAsync(e.Message.Photo[e.Message.Photo.Count() - 1].FileId);
                DownloadFile(e.Message.Photo[e.Message.Photo.Count() - 1].FileId,
                    file.FilePath.Replace("photos/", "DownloadFiles/"));

                //var photo = bot.GetFileAsync(e.Message.Photo[e.Message.Photo.Count() - 1].FileId);
                //var download_url = @$"https://api.telegram.org/file/bot{token}/" + photo.Result.FilePath;
                //using (WebClient client = new WebClient())
                //{
                //    client.DownloadFile(new Uri(download_url),
                //        @"DownloadFiles\" + photo.Result.FilePath.Replace(@"photos/", ""));
                //}
                //Console.WriteLine(e.Message.Document.FileName);
            }

            if (e.Message.Type == MessageType.Video)
            {
                var file = await bot.GetFileAsync(e.Message.Video.FileId);
                DownloadFile(e.Message.Video.FileId, file.FilePath.Replace("videos/", "DownloadFiles/"));
                //var video = await bot.GetFileAsync(e.Message.Video[e.Message.Video.Count() - 1].FileId);
            }

            if (e.Message.Type == MessageType.Audio)
            {
                DownloadFile(e.Message.Audio.FileId, @"DownloadFiles\" + e.Message.Audio.FileName);
            }

            if (e.Message.Type == MessageType.Voice)
            {
                var file = await bot.GetFileAsync(e.Message.Voice.FileId);
                DownloadFile(e.Message.Voice.FileId, file.FilePath.Replace("voice/", "DownloadFiles/"));
            }
            //Подписка на событие при нажатии кнопки inlineButtons
            bot.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
            {
                var message = ev.CallbackQuery.Message;
                if (ev.CallbackQuery.Data.Equals("msg0000.wav"))
                {
                    await bot.SendTextMessageAsync(chatId, "msg0000.wav");
                }
                else if (ev.CallbackQuery.Data.Equals(""))
                {

                }
            };
            
            KeyboardMarkups();
            Thread.Sleep(50);
        }

        private static async void DownloadFile(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);
            using (FileStream fs = new(path, FileMode.OpenOrCreate))
            {
                await bot.DownloadFileAsync(file.FilePath, fs);
            }
        }

        private static IReplyMarkup KeyboardMarkups()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        new KeyboardButton() { Text = "/start" }, { new KeyboardButton { Text = "Ещё функция:)" } },
                        //new KeyboardButton() { Text = "Список файлов" }, { new KeyboardButton { Text = "Назад" } }
                    },
                    new List<KeyboardButton>
                    {
                        new KeyboardButton() { Text = "Список файлов" }, { new KeyboardButton { Text = "Назад" } }
                        //    new KeyboardButton() { Text = "Привет", }, { new KeyboardButton { Text = "Бна" } },
                        //    new KeyboardButton() { Text = "WWw" }, { new KeyboardButton { Text = "Hi" } }
                    }
                    
                    //new List<KeyboardButton>
                    //{
                    //    new KeyboardButton() { Text = "123", }, { new KeyboardButton { Text = "ghjghj" } },
                    //    new KeyboardButton() { Text = "Список файлов" }, { new KeyboardButton { Text = "Назад" } }
                    //}
                    
                },
                ResizeKeyboard = true
                
            }; 
        }

        private static void FilesList()
        {
            int count = 0;
            string[] files = Directory.GetFiles("DownloadFiles");
            foreach (string s in files)
            {
                var fInfo = new FileInfo(s);
                Console.WriteLine($"{++count}. {fInfo.Name}");
            }
        }

        private static async void CallbackMarkup(long chatId)
        {
            string[] files = Directory.GetFiles("DownloadFiles");
            foreach (string s in files)
            {
                var fInfo = new FileInfo(s);
                //Console.WriteLine($"{++count}. {fInfo.Name}");
                //await bot.SendTextMessageAsync(chatId, $"{fInfo.Name}");
                //CallbackMarkup(fInfo.Name, chatId);
                 GetInlineKeyboard();
                InlineKeyboardMarkup inlineKeyboard = new(new[]
                {
                    // first row
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: fInfo.Name, callbackData: fInfo.Name),
                        //InlineKeyboardButton.WithCallbackData(text: "1.2", callbackData: "12"),
                    },
                    // second row
                    //new[]
                    //{
                    //    InlineKeyboardButton.WithCallbackData(text: "2.1", callbackData: "21"),
                    //    InlineKeyboardButton.WithCallbackData(text: "2.2", callbackData: "22"),
                    //},
                });

            Message sentMessage = await bot.SendTextMessageAsync(
                chatId: chatId,
                "Так" ,
                replyMarkup: inlineKeyboard
            
            );
            }
        }

        private static IReplyMarkup GetInlineKeyboard()
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Заказать", CallbackData = "Order" });
        }
    }
}