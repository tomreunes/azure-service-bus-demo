using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace AzureServiceBusDemo
{
    public class DeadletterForwarderDemo : IDemo
    {
        private MessageReceiver queueReceiver;
        
        public void Execute()
        {
            FillQueue();

            ReceiveFromQueue();
        }

        private void FillQueue()
        {
            var sender = new MessageSender(Configuration.ServiceBusConnectionString, "deadletterForwarder");
            var message = new Message();
            sender.SendAsync(message).Wait();
        }
        private void ReceiveFromQueue()
        {
            Logger.Log("Receiver started. Enter 'ok' to stop");
            ReceiveQueue();
            while (Console.ReadLine() != "ok") { }

            queueReceiver.CloseAsync().Wait();
        }
        
        private void ReceiveQueue()
        {
            queueReceiver = new MessageReceiver(Configuration.ServiceBusConnectionString, "deadletterForwarder");

            RegisterHandler(queueReceiver);
        }

        private void RegisterHandler(MessageReceiver receiver)
        {
            receiver.RegisterMessageHandler(async (message, token) =>
            {
                Logger.Log($"Message {message.MessageId} received (label: {message.Label})");
                await receiver.DeadLetterAsync(message.SystemProperties.LockToken, "because I want to", "because I can");
                Logger.Log("Deadlettered message");
            }, new MessageHandlerOptions(args => Task.CompletedTask)
            {
                MaxConcurrentCalls = 2,
                AutoComplete = false
            });
        }
    }
}