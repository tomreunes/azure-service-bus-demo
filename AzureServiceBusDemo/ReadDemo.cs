using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;

namespace AzureServiceBusDemo
{
    public class ReadDemo : IDemo
    {
        private MessageReceiver queueReceiver;
        private MessageReceiver subscriptionReceiver;

        private const string QueueName = "demo1/queue";
        private const string TopicName = "demo1/topic2";
        private const string SubscriptionName = "s2";
        public void Execute()
        {
            ReceiveFromQueue();

            Logger.Log("Enter to receive subscription messages");
            Console.Read();
            ReceiveFromSubscription();
        }

        private void ReceiveFromQueue()
        {
            Logger.Log("Receiver started. Enter 'ok' to stop");
            ReceiveQueue();
            while (Console.ReadLine() != "ok") { }

            queueReceiver.CloseAsync().Wait();
        }

        private void ReceiveFromSubscription()
        {
            Logger.Log("Receiver started. Enter 'ok' to stop");
            ReceiveSubscription();
            while (Console.ReadLine() != "ok") { }

            subscriptionReceiver.CloseAsync().Wait();
        }

        private void ReceiveQueue()
        {
            queueReceiver = new MessageReceiver(Configuration.ServiceBusConnectionString, QueueName, ReceiveMode.ReceiveAndDelete, RetryPolicy.Default);

            RegisterHandler(queueReceiver);
        }

        private void RegisterHandler(MessageReceiver receiver)
        {
            receiver.RegisterMessageHandler(async (message, token) =>
            {
                Logger.Log($"Message {message.MessageId} received (label: {message.Label})");
                if (message.Body != null && message.Body.Length > 1)
                {
                    var content = JsonConvert.DeserializeObject<DemoObject>(Encoding.UTF8.GetString(message.Body));
                    Logger.Log(content.Company);
                }

                await Task.Delay(1000, token);
                Logger.Log($"Message {message.MessageId} handled");
            }, new MessageHandlerOptions(args => Task.CompletedTask)
            {
                MaxConcurrentCalls = 2,
                AutoComplete = true
            });
        }

        private void ReceiveSubscription()
        {
            subscriptionReceiver = new MessageReceiver(Configuration.ServiceBusConnectionString, EntityNameHelper.FormatSubscriptionPath(TopicName, SubscriptionName), ReceiveMode.ReceiveAndDelete, RetryPolicy.Default);

            RegisterHandler(subscriptionReceiver);
        }
    }
}