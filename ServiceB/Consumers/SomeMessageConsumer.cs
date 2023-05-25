using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MassTransit;

namespace ServiceB.Consumers
{
    public class SomeMessageConsumer : IConsumer<SomeMessage>
    {
        private readonly ILogger<SomeMessageConsumer> _logger;

        public SomeMessageConsumer(ILogger<SomeMessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<SomeMessage> context)
        {
            var message = context.Message;
            //do something with the message
            _logger.LogWarning($"Message content: {message.Name}, {message.Email}.");
            return Task.CompletedTask;
        }
    }
}