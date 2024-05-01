namespace MangoFood.Services.RewardAPI.Messaging
{
    public interface IAzureBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
