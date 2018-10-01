using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace AzureServiceBusDemo
{
    public class SubscriptionFilterDemo : IDemo
    {
        private const string TopicName = "demo4/topic";
        public void Execute()
        {
            FillQueue();
        }

        private void FillQueue()
        {
            var sender = new MessageSender(Configuration.ServiceBusConnectionString, TopicName);
            sender.SendAsync(new List<Message>
            {
                new Message
                {
                    UserProperties =
                    {
                        { "Value", 20 },
                        { "Name" , "Jos" }
                    }
                },
                new Message
                {
                    UserProperties =
                    {
                        { "Value", 100 },
                        { "Name" , "Bernard" }
                    }
                },
                new Message
                {
                    UserProperties =
                    {
                        { "Value", 100 },
                        { "Name" , "Jos" }
                    }
                },
                new Message
                {
                    UserProperties =
                    {
                        { "Value", 20 },
                        { "Name" , "Jos" }
                    }
                },
                new Message
                {
                    UserProperties =
                    {
                        { "Value", 20 },
                        { "Name" , "Mark" }
                    }
                },
            }).Wait();
        }
    }
}