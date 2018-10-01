using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace AzureServiceBusDemo
{
    public class SchedulingDemo : IDemo
    {
        public void Execute()
        {

            var sender = new MessageSender(Configuration.ServiceBusConnectionString, "scheduling");
            var scheduledMessage = new Message
            {
                ScheduledEnqueueTimeUtc = DateTime.UtcNow.AddSeconds(30)
            };
            sender.SendAsync(scheduledMessage).Wait();
            for (var i = 0; i < 10; i++)
            {
                var message = new Message();
                sender.SendAsync(message).Wait();
            }
        }
    }
}
