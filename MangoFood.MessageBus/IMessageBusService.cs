

namespace MangoFood.MessageBus
{
    public interface IMessageBusService
    {
        Task PublishMessage(object message, string topic_queue_Name);
    }
}
