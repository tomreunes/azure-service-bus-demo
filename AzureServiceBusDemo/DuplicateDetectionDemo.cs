using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace AzureServiceBusDemo
{
    public class DuplicateDetectionDemo : IDemo
    {
        public void Execute()
        {
            var sender = new MessageSender(Configuration.ServiceBusConnectionString, "duplicateDetection");
            for (var i = 0; i < 10; i++)
            {
                var message = new Message
                {
                    MessageId = "uniqueMessageId"
                };
                sender.SendAsync(message).Wait();
            }
        }
    }
}
