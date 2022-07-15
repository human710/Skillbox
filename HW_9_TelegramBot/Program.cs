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
                TelegramBotClass bot = new TelegramBotClass(token: "");
                bot.GetUpdates();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
        }
    }
}