using SharedAssembly;
using SharedAssembly.Models;
using SharedAssembly.RabbitMQ;

var configBuilder = new LocalConfigurationBuilder();
var config = configBuilder.BuildJson("appsettings.json");

var rabbitMqSetupModel = new RabbitMqSetupModel(
    RabbitMQConfig.DefaultUri, RabbitMQConfig.DataCaptureExchange,
    RabbitMQConfig.DataCaptureQueue, RabbitMQConfig.DataCaptureRoutingKey
);
var resultFileInfo = new ResultFileInfo(
    config["dataPath"],
    config["jpg"]
);
using var rabbitMqService = new RabbitMQService(rabbitMqSetupModel);
rabbitMqService.Setup();

Console.WriteLine("Init completed");

rabbitMqService.RegisterDataExchangeConsumer();

Console.WriteLine("Consumer is registered. Awaiting messages...");

Console.ReadKey();