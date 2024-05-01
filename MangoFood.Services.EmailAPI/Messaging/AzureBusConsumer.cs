using Azure.Messaging.ServiceBus;
using MangoFood.Services.EmailAPI.Models.DTO;
using MangoFood.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace MangoFood.Services.EmailAPI.Messaging
{
    public class AzureBusConsumer : IAzureBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedTopicSubcription;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private readonly ServiceBusProcessor _emailCartProcessor;
        private readonly ServiceBusProcessor _registerUserProcessor;
        private readonly ServiceBusProcessor _orderCreatedProcessor;

        public AzureBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString")!;
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue")!;
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")!;
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;
            orderCreatedTopicSubcription = _configuration
                .GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription")!;

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
            _orderCreatedProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedTopicSubcription);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestRecieved;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;

            _orderCreatedProcessor.ProcessMessageAsync += OnOrderCreatedRecieved;
            _orderCreatedProcessor.ProcessErrorAsync += ErrorHandler;

            _registerUserProcessor.ProcessErrorAsync += ErrorHandler; 
            _registerUserProcessor.ProcessMessageAsync += OnRegisterUserRecieved;

            await _emailCartProcessor.StartProcessingAsync();
            await _registerUserProcessor.StartProcessingAsync();
            await _orderCreatedProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();

            await _orderCreatedProcessor.StopProcessingAsync();
            await _orderCreatedProcessor.DisposeAsync();

            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnOrderCreatedRecieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var messageObj = JsonConvert.DeserializeObject<RewardDTO>(body);
            //TODO - ttry to log email
            try
            {
                await _emailService.LogOrderPlaced(messageObj);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnEmailCartRequestRecieved(ProcessMessageEventArgs args)
        {
            //recieve message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var messageObj = JsonConvert.DeserializeObject<ShoppingCartDTO>(body);
            //TODO - ttry to log email
            try
            {
                await _emailService.EmailAndCatalog(messageObj);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnRegisterUserRecieved(ProcessMessageEventArgs args)
        {
            //recieve message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var messageObj = JsonConvert.DeserializeObject<string>(body);
            //TODO - ttry to log email
            try
            {
                await _emailService.UserRegisteredCatalog(messageObj);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
