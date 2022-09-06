using ConsoleApp1;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
class Program
{
    

    

    static void Main(string[] args)
    {
        Logger logger;
        logger = new Logger();
        Thread loggerThread = new Thread(new ThreadStart(logger.Start));
        loggerThread.Start();

        

        Console.ReadKey();
        logger.Stop();
        Console.WriteLine("Im closing");
        Console.ReadKey();
    }
}