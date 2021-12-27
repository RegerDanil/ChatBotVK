using System;
using Chat_Bot_Config;

namespace Vk_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            new Bot(VkConfig.Instance).Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

        }
    }
}
