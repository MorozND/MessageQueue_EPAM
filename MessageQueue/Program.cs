using SharedAssembly.Models;
using SharedAssembly.RabbitMQ;

var rabbitMqSetupModel = new RabbitMqSetupModel(
    RabbitMQConfig.DefaultUri, RabbitMQConfig.DataCaptureExchange,
    RabbitMQConfig.DataCaptureQueue, RabbitMQConfig.DataCaptureRoutingKey
);
using var rabbitMqService = new RabbitMQService(rabbitMqSetupModel);
rabbitMqService.Setup();

Console.WriteLine("Init completed");

Console.ReadKey();