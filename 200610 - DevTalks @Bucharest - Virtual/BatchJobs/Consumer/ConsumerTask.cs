using Common;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    public class ConsumerTask
    {
        private readonly QueueClient _sender = null;
        private readonly QueueClient _receiver = null;

        public ConsumerTask()
        {
            this._receiver = new QueueClient(Constants.ServiceBusConnection,Constants.TodoQueueName);
            this._sender = new QueueClient(Constants.ServiceBusConnection, Constants.ResultsQueueName);
        }

        public void Run()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            _receiver.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.WriteLine("Reading messages");
            while (true)
            {
                //This emulates the CPU processing time
                Thread.Sleep(1000);
            }
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var text = Encoding.UTF8.GetString(message.Body).Split(':');
            var numbers = new
            {
                A = int.Parse(text[0]),
                B = int.Parse(text[1])
            };
            var res = numbers.A + numbers.B;
            await Task.Delay(1000, token);
            Console.WriteLine($"Processed: {numbers.A}+{numbers.B}={res} with id: {message.CorrelationId}");

            await _sender.SendAsync(new Message(Encoding.UTF8.GetBytes(res.ToString())) { CorrelationId = message.CorrelationId });
            await _receiver.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine("AMEN");
            return Task.CompletedTask;
        }
    }
}
