

using MangoFood.Services.RewardAPI.Messaging;

namespace MangoFood.Services.RewardAPI.StartupExtensions
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureBusConsumer ServiceBusConsumer {  get; set; }
        public static IApplicationBuilder UseAzureBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureBusConsumer>();
            var hostApplicationService = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationService.ApplicationStarted.Register(OnStart);
            hostApplicationService.ApplicationStopping.Register(OnStop);

            return app;
        }

        private static void OnStart()
        {
            ServiceBusConsumer.Start();
        }

        private static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }
    }
}
