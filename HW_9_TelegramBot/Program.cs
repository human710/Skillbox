using System;

namespace HW_9_TelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //new TelegrammClass();
            try
            {
                TelegramBotClass bot = new TelegramBotClass(token: "5292001260:AAECjsY3A-tKHtedpt76BlR68uprUnicDr8");
                bot.GetUpdates();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
        }
    }
}