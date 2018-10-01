using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace AzureServiceBusDemo
{
    public class ExpirationDemo : IDemo
    {
        public void Execute()
        {
            var sender = new MessageSender(Configuration.ServiceBusConnectionString, "expiration");
            var message = new Message
            {
                MessageId = "noTimeToLive"
            };
            sender.SendAsync(message).Wait();

            message = new Message
            {
                MessageId = "longerTimeToLive",
                TimeToLive = new TimeSpan(0, 1, 0)
            };
            sender.SendAsync(message).Wait();

            message = new Message
            {
                MessageId = "shorterTimeToLive",
                TimeToLive = new TimeSpan(0, 0, 2)
            };
            sender.SendAsync(message).Wait();
        }
    }
}
