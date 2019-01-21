using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using WordPressSharp;
using WordPressSharp.Models;

namespace TelegramBotToWordPress
{
    class Program
    {
        private static readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);
        private static TelegramBotClient _bot;
        private static ISimpleWordPressClient _simpleWordPressClient;
        static void Main(string[] args)
        {
            Console.Out.WriteLine("Starting TelegramBot To WordPress");

            try
            {
                var configurationRoot = new ConfigurationBuilder().AddEnvironmentVariables().Build();
                var baseUrl = configurationRoot["wp-baseurl"];
                var blogId = Convert.ToInt32(configurationRoot["wp-blogid"]);
                var username = configurationRoot["wp-username"];
                var password = configurationRoot["wp-password"];
                var parentId = configurationRoot["wp-parentid"];
                var author = configurationRoot["wp-author"];
                var botApiKey = configurationRoot["bot-apikey"];

                _simpleWordPressClient = new SimpleWordPressClient(baseUrl, username, password, blogId, author, parentId);
                _bot = new TelegramBotClient(botApiKey);
            }
            catch (Exception exception)
            {
                Console.Out.WriteLine(exception);
                throw;
            }

            var me = _bot.GetMeAsync().Result;
            _bot.OnMessage += BotOnMessageReceived;
            //_bot.OnMessageEdited += BotOnMessageReceived;
            //Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            //Bot.OnInlineQuery += BotOnInlineQueryReceived;
            //Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            _bot.OnReceiveError += BotOnReceiveError;

            _bot.StartReceiving(Array.Empty<UpdateType>());
            Console.Out.WriteLine($"Start listening for @{me.Username}");


            // Wait
            WaitHandle.WaitOne();
            _bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text || message.ForwardFromChat != null) return;
            var writer = $"{message.From.FirstName} {message.From.LastName} {message.From.Username}";
            var postText = $"{message.Text}{Environment.NewLine}{writer}";
            var id = await _simpleWordPressClient.SendForumPost(postText);
            Console.Out.WriteLine(id);
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.Out.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }

    }
}
