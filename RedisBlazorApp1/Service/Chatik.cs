using StackExchange.Redis;
using Microsoft.AspNetCore.Components.Web;
using RedisBlazorApp1.Components.Pages;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;


namespace RedisBlazorApp1.Service
{
    public class Chatik : EventArgs
    {
        private bool inited;
        private static ConnectionMultiplexer redis;
        private static ISubscriber subscriber;
        private List<string> messages;

        public event EventHandler Handler;
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
                  
                    Handler?.Invoke(null, null);
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
