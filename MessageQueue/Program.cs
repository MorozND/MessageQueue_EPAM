
using SharedAssembly.RabbitMQ;

using var rabbitMqService = new RabbitMQService(RabbitMQConsts.DefaultUri);
rabbitMqService.SetupExchangeAndQueue(
    RabbitMQConsts.DataCaptureExchange,
    RabbitMQConsts.DataCaptureQueue,
    RabbitMQConsts.DataCaptureRoutingKey
);

Console.WriteLine("Init completed");

Console.ReadKey();