using Common;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Viewer
{
    public class ViewerTask
    {
        private readonly QueueClient _receiver = null;

        public ViewerTask()
        {
            this._receiver = new QueueClient(Constants.ServiceBusConnection,Constants.ResultsQueueName);
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
            Console.Read();
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var text = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Result: {text} for {message.CorrelationId}");
            await _receiver.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine("AMEN");
            return Task.CompletedTask;
        }
    }
}
