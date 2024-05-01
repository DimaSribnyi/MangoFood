namespace MangoFood.Services.EmailAPI.Messaging
{
    public interface IAzureBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
