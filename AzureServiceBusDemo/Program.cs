using System;
using System.Linq;

namespace AzureServiceBusDemo
{
    class Program
    {
        static void Main()
        {
            var demos = new IDemo[]
            {
                new WriteDemo(),
                new ReadDemo(),
                new PeekLockDemo(),
                new SubscriptionFilterDemo(),
                new DeadletterForwarderDemo(), 
                new DuplicateDetectionDemo(), 
                new SchedulingDemo(), 
                new ExpirationDemo(), 
                new SessionsDemo(), 
            };

            while (true)
            {
                Logger.Log("-------------------------------------------", ConsoleColor.Yellow);
                foreach (var i in Enumerable.Range(1, demos.Length))
                {
                    Logger.Log($"[{i}] {demos[i-1].GetType().Name}", ConsoleColor.Yellow);
                }
                Logger.Log("");
                Logger.Log("Please enter the number of the demo you want to display: ", ConsoleColor.Yellow);
                var demoId = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(demoId))
                {
                    demoId = Console.ReadLine();
                }
                var index = int.Parse(demoId);
                var demoToExecute = demos[index - 1];
                Logger.Log($"Executing demo {demoToExecute.GetType().Name}", ConsoleColor.Yellow);
                Logger.Log("-------------------------------------------", ConsoleColor.Yellow);
                demoToExecute.Execute();
                Logger.Log("");
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
