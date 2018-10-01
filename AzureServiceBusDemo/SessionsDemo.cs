using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace AzureServiceBusDemo
{
    public class SessionsDemo : IDemo
    {
        private static MessageReceiver messageReceiver;

        public void Execute()
        {
            StartWorkers();
            Task.WaitAll(
                StartSessionSender("A", ConsoleColor.Red), 
                StartSessionSender("B", ConsoleColor.Yellow),
                StartSessionSender("C", ConsoleColor.Green)
                );
            Logger.Log("Receiver started. Enter 'ok' to stop");
            while (Console.ReadLine() != "ok") { }

            messageReceiver.CloseAsync().Wait();
        }
        private static async Task StartSessionSender(string sessionId, ConsoleColor color)
        {
            var message = new Message
            {
                ReplyToSessionId = sessionId,
                ReplyTo = "sender"
            };

            var sender = new MessageSender(Configuration.ServiceBusConnectionString, "worker");

            await sender.SendAsync(message);
            Logger.Log($"[{sessionId}] write to worker queue", color);

            var client = new SessionClient(Configuration.ServiceBusConnectionString, "sender");
            var session = await client.AcceptMessageSessionAsync(sessionId, new TimeSpan(0, 1, 0));
            while (true)
            {
                if (session.IsClosedOrClosing) return;
                var receivedMessage = await session.ReceiveAsync();
                if (receivedMessage == null) continue;
                Logger.Log($"[{sessionId}] Received message {receivedMessage.UserProperties["result"]}", color);
                await session.CompleteAsync(receivedMessage.SystemProperties.LockToken);
            }
        }

        private static void StartWorkers()
        {
            messageReceiver = new MessageReceiver(Configuration.ServiceBusConnectionString, "worker");
            messageReceiver.RegisterMessageHandler(async (message, token) =>
            {
                var sender = new MessageSender(Configuration.ServiceBusConnectionString, message.ReplyTo);
                var response = new Message
                {
                    SessionId = message.ReplyToSessionId,
                    UserProperties = { { "result", message.ReplyToSessionId + 1 } }
                };
                Logger.Log($"Worker handled 1 {message.ReplyToSessionId}. Response: {message.ReplyToSessionId + 1}");
                await sender.SendAsync(response);
                await Task.Delay(new Random().Next(500, 2000), token);
                Logger.Log($"Worker handled 2 {message.ReplyToSessionId}. Response: {message.ReplyToSessionId + 2}");
                var clone = response.Clone();
                clone.UserProperties["result"] = message.ReplyToSessionId + 2;
                await sender.SendAsync(clone);

                await messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
            }, new MessageHandlerOptions(args => Task.CompletedTask)
            {
                MaxConcurrentCalls = 2
            });
        }
    }
}
