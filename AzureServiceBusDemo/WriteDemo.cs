using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;

namespace AzureServiceBusDemo
{
    public class WriteDemo : IDemo
    {
        private const string QueueName = "demo1/queue";
        private const string Topic1Name = "demo1/topic1";
        private const string Topic2Name = "demo1/topic2";
        public void Execute()
        {
            WriteQueueUsingQueueClient().Wait();
            
            WriteTopicUsingTopicClient1().Wait();
            
            WriteTopicUsingTopicClient2().Wait();

            WriteQueueUsingMessageSender().Wait();

            WriteTopicUsingMessageSender().Wait();
        }

        private async Task WriteQueueUsingQueueClient()
        {
            var client = new QueueClient(Configuration.ServiceBusConnectionString, QueueName);
            var obj = new DemoObject
            {
                Company = "Euricom",
                Location = "Malaga"
            };
            var serialized = JsonConvert.SerializeObject(obj);
            var message = new Message(Encoding.UTF8.GetBytes(serialized))
            {
                Label = "Welcome"
            };
            await client.SendAsync(message);
            Logger.Log("Sent message using QueueClient");
        }

        private async Task WriteTopicUsingTopicClient1()
        {
            var client = new TopicClient(Configuration.ServiceBusConnectionString, Topic1Name);
            var obj = new DemoObject
            {
                Company = "Euricom",
                Location = "Malaga"
            };
            var serialized = JsonConvert.SerializeObject(obj);
            var message = new Message(Encoding.UTF8.GetBytes(serialized))
            {
                Label = "Welcome"
            };
            await client.SendAsync(message);
            Logger.Log("Sent message using TopicClient");
        }

        private async Task WriteTopicUsingTopicClient2()
        {
            var client = new TopicClient(Configuration.ServiceBusConnectionString, Topic2Name);
            var obj = new DemoObject
            {
                Company = "Euricom",
                Location = "Malaga"
            };
            var serialized = JsonConvert.SerializeObject(obj);
            var message = new Message(Encoding.UTF8.GetBytes(serialized))
            {
                Label = "Welcome"
            };
            await client.SendAsync(message);
            Logger.Log("Sent message using TopicClient");
        }
        private async Task WriteQueueUsingMessageSender()
        {
            await WriteUsingMessageSender(QueueName);
        }
        private async Task WriteTopicUsingMessageSender()
        {
            await WriteUsingMessageSender(Topic2Name);
        }

        private async Task WriteUsingMessageSender(string entityPath)
        {
            var sender = new MessageSender(Configuration.ServiceBusConnectionString, entityPath);
            var message = new Message
            {
                UserProperties = { { "propertyName", "propertyValue" } }
            };
            await sender.SendAsync(message);
            Logger.Log("Sent message using MessageSender");
        }
    }
}