using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp1
{
   
    class Logger
    {
        static ITelegramBotClient bot = new TelegramBotClient("5734833126:AAH2we9n2qorE11_dTOCoAxGdSq5b2AzYlA");

        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        public Logger()
        {
            watcher = new FileSystemWatcher("C:\\Temp");
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message == null) return;
                Console.WriteLine("Из чата " + message.Chat.Id + " пришло сообщение:\n" +
                    message.Text);
                await bot.SendTextMessageAsync(-1001679747659, "Он написал: \"" + message.Text + "\"");
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine("Ошипка");
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }
        // переименование файлов
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "переименован в " + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
        }
        // изменение файлов
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "изменен";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "создан";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        // удаление файлов
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "удален";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        async public void Write(string text)
        {
            await bot.SendTextMessageAsync(-1001679747659, text);
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter("C:\\templog.txt", true))
                {
                    String message = String.Format("{0} файл {1} был {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent);
                    Write(message);
                    writer.WriteLine(message);
                    writer.Flush();
                }
            }
        }
    }
}

