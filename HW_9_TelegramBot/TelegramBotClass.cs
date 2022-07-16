using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace HW_9_TelegramBot
{
    public class TelegramBotClass
    {
        private string _token;
        static TelegramBotClient _client;

        public TelegramBotClass(string token)
        {
            this._token = token;
        }

        internal void GetUpdates()
        {
            _client = new TelegramBotClient(_token);
            var me = _client.GetMeAsync().Result;
            if (me != null && !string.IsNullOrEmpty(me.Username))
            {
                int offset = 0;
                while (true)
                {
                    try
                    {
                        var updates = _client.GetUpdatesAsync(offset).Result;
                        if (updates != null && updates.Count() > 0)
                        {
                            foreach (var update in updates)
                            {
                                processUpdate(update);
                                offset = update.Id + 1;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    Thread.Sleep(500);
                }
            }
        }

        private async void processUpdate(Update update)
        {
            if (update.Message != null)
            {
                if (update.Message.Type == MessageType.Document)
                {
                    if (!Directory.Exists("DownloadFiles"))
                        Directory.CreateDirectory("DownloadFiles");
                    DownloadFile(update.Message.Document.FileId, @"DownloadFiles\" + update.Message.Document.FileName);
                }

                if (update.Message.Type == MessageType.Photo)
                {
                    var file = await _client.GetFileAsync(update.Message.Photo[update.Message.Photo.Count() - 1]
                        .FileId);
                    DownloadFile(update.Message.Photo[update.Message.Photo.Count() - 1].FileId,
                        file.FilePath.Replace("photos/", "DownloadFiles/"));
                }

                if (update.Message.Type == MessageType.Video)
                {
                    var file = await _client.GetFileAsync(update.Message.Video.FileId);
                    DownloadFile(update.Message.Video.FileId, file.FilePath.Replace("videos/", "DownloadFiles/"));
                }

                if (update.Message.Type == MessageType.Audio)
                {
                    DownloadFile(update.Message.Audio.FileId, @"DownloadFiles\" + update.Message.Audio.FileName);
                }

                if (update.Message.Type == MessageType.Voice)
                {
                    var file = await _client.GetFileAsync(update.Message.Voice.FileId);
                    DownloadFile(update.Message.Voice.FileId, file.FilePath.Replace("voice/", "DownloadFiles/"));
                }
            }

            switch (update.Type)
            {
                    
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    var text = update.Message.Text;
                    if (!Directory.Exists("DownloadFiles"))
                    {
                        Directory.CreateDirectory("DownloadFiles");
                    }
                    switch (text)
                    {
                        case "/start":
                            await _client.SendTextMessageAsync(update.Message.Chat.Id,
                                "Приветствую! Данный чат бот позволяет принимать, хранить и скачивать файлы. " +
                                "Функциональные возможности будут расширяться и дорабатываться.\nНажмите кнопку 'Список файлов' " +
                                "чтобы посмореть доступные для загрузки файлы", replyMarkup: GetButtons());
                            break;
                        case "Список файлов":
                            string[] files = Directory.GetFiles("DownloadFiles");
                            int count = 1;
                            foreach (string s in files)
                            {
                                var fInfo = new FileInfo(s);

                                await _client.SendTextMessageAsync(update.Message.Chat.Id, $"{count}. {fInfo.Name}",
                                    replyMarkup: GetInlineButtons(fInfo.Name));

                                count++;
                            }

                            break;
                    }

                    break;
                case UpdateType.CallbackQuery:
                    using (var stream = File.OpenRead($"DownloadFiles/{update.CallbackQuery.Data}"))
                    {
                        InputOnlineFile iof = new InputOnlineFile(stream);
                        iof.FileName = update.CallbackQuery.Data;
                        var r = _client.SendDocumentAsync(update.CallbackQuery.Message.Chat.Id, iof).Result;
                    }

                    break;
                default:
                    Console.WriteLine(update.Type + "Not implemented!");
                    break;
            }
        }

        private IReplyMarkup GetInlineButtons(string name)
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Скачать", CallbackData = name });
        }

        private IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        new KeyboardButton() { Text = "/start" }, { new KeyboardButton { Text = "Список файлов" } },
                    },
                    //На случай если понадобится добавить ряд кнопок :)
                    //new List<KeyboardButton>
                    //{
                    //    new KeyboardButton() { Text = "Список файлов" }, { new KeyboardButton { Text = "Назад" } }
                    //}
                },
                ResizeKeyboard = true
            };
        }

        private static async void DownloadFile(string fileId, string path)
        {
            var file = await _client.GetFileAsync(fileId);
            using (FileStream fs = new(path, FileMode.OpenOrCreate))
            {
                await _client.DownloadFileAsync(file.FilePath, fs);
            }
        }
    }
}