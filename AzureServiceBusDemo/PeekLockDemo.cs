using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace AzureServiceBusDemo
{
    public class PeekLockDemo : IDemo
    {
        private MessageReceiver queueReceiver;

        private const string QueueName = "demo3/queue";
        public void Execute()
        {
            FillQueue();

            ReceiveFromQueue();
        }

        private void FillQueue()
        {
            var sender = new MessageSender(Configuration.ServiceBusConnectionString, QueueName);
            sender.SendAsync(new List<Message>
            {
                new Message
                {
                    Label = "Complete"
                },
                new Message
                {
                    Label = "Abandon"
                },
                new Message
                {
                    Label = "Deadletter"
                },
                new Message
                {
                    Label = "Defer"
                },
            }).Wait();
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
            queueReceiver = new MessageReceiver(Configuration.ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock, RetryPolicy.Default);

            RegisterHandler(queueReceiver);
        }

        private void RegisterHandler(MessageReceiver receiver)
        {
            receiver.RegisterMessageHandler(async (message, token) =>
            {
                Logger.Log($"Message {message.MessageId} received (label: {message.Label})");
                switch (message.Label)
                {
                    case "Complete":
                        await receiver.CompleteAsync(message.SystemProperties.LockToken);
                        break;
                    case "Abandon":
                        await receiver.AbandonAsync(message.SystemProperties.LockToken);
                        break;
                    case "Deadletter":
                        await receiver.DeadLetterAsync(message.SystemProperties.LockToken, "reason", "descr");
                        break;
                    case "Defer":
                        await receiver.DeferAsync(message.SystemProperties.LockToken);
                        break;
                }
            }, new MessageHandlerOptions(args => Task.CompletedTask)
            {
                MaxConcurrentCalls = 2,
                AutoComplete = false
            });
        }
    }
}