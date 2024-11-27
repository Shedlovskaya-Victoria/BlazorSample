using StackExchange.Redis;
using Microsoft.AspNetCore.Components.Web;
using RedisBlazorApp1.Components.Pages;


namespace RedisBlazorApp1.Service
{
    public class Chatik
    {
        private bool inited;
        private static ConnectionMultiplexer redis;
        private static ISubscriber subscriber;
        private List<string> messages;

        public delegate void MessagesHadler(string message);
        MessagesHadler? hadler;

        public event MessagesHadler handler
        {
            add
            {
                hadler += value;
                Console.WriteLine($"{value.Method.Name} добавлен");
            }
            remove
            {
                hadler -= value;
                Console.WriteLine($"{value.Method.Name} удален");
            }
        }

        public event EventHandler OnNewMessage;
        public Chatik()
        {
            messages = new List<string>();
            inited = false;
            Initialized();
        }

        public void Initialized()
        {
            if (!inited)
            {
                inited = true;
                //создаем соединение 1 раз в синглоне
                redis = ConnectionMultiplexer.Connect("192.168.200.44");
                //получаем интерфейс работы с подписчиками 
                subscriber = redis.GetSubscriber();
                //создание подписки на канал messages
                subscriber.Subscribe("messages", (shannel, message) =>
                {
                    //тут обработка сообщений, для блецзор-страницы можно 
                    // добавить событие с аргуаментом сообщением
                    messages.Add($"from channel {shannel} is message: {message}");
                    OnNewMessage?.Invoke(this, null);
                });
            }
        }

        public void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;


            subscriber.Publish("messages", message);
        }
        public List<string> GetMessages()
        {
            return messages;
        }
    }
}
