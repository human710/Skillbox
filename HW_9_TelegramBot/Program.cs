using System;

namespace HW_9_TelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TelegramService bot = new TelegramService(token: ""); //Здесь был токен :)
                bot.GetUpdates();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
        }
    }
}