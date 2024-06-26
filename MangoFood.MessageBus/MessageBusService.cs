﻿

using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace MangoFood.MessageBus
{
    public class MessageBusService : IMessageBusService
    {
        private string connectionString = SD.connectionString;
        public async Task PublishMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(topic_queue_Name);

            var jsonMessage = JsonConvert.SerializeObject(message);

            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);
        }
    }
}
