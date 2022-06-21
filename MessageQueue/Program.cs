
using SharedAssembly.RabbitMQ;

using var rabbitMqService = new RabbitMQService(RabbitMQConfig.DefaultUri);
rabbitMqService.SetupExchangeAndQueue(
    RabbitMQConfig.DataCaptureExchange,
    RabbitMQConfig.DataCaptureQueue,
    RabbitMQConfig.DataCaptureRoutingKey
);

Console.WriteLine("Init completed");

Console.ReadKey();