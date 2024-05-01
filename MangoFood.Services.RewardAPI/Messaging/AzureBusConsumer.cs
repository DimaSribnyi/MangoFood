using Azure.Messaging.ServiceBus;
using MangoFood.Services.RewardAPI.Models.DTO;
using MangoFood.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace MangoFood.Services.RewardAPI.Messaging
{
    public class AzureBusConsumer : IAzureBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedTopicSubcription;
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;

        private readonly ServiceBusProcessor _rewardProcessor;

        public AzureBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            _rewardService = rewardService;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString")!;
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;
            orderCreatedTopicSubcription = _configuration
                .GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription")!;

            var client = new ServiceBusClient(serviceBusConnectionString);
            _rewardProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedTopicSubcription);
        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardRecieved;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnNewOrderRewardRecieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var messageObj = JsonConvert.DeserializeObject<RewardDTO>(body);
            //TODO - ttry to log email
            try
            {
                await _rewardService.UpdateRewards(messageObj);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
