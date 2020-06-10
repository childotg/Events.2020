using Common;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Producer
{
    public class ProducerTask
    {
        private readonly Random _random;
        public ProducerTask()
        {
            this._random = new Random(DateTime.Now.Millisecond);
        }
        public void Run()
        {
            var queueClient = new QueueClient(Constants.ServiceBusConnection,Constants.TodoQueueName);

            while (true)
            {
                var message = new Message(Encoding.UTF8.GetBytes($"{_random.Next(1000)}:{_random.Next(1000)}"))
                {
                    CorrelationId = Guid.NewGuid().ToString()
                };
                Console.WriteLine($"Enqueuing message {message.CorrelationId}...");
                queueClient.SendAsync(message).GetAwaiter().GetResult();
            }            
        }
    }
}
