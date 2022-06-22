using SharedAssembly;
using SharedAssembly.Models;
using SharedAssembly.RabbitMQ;

var configBuilder = new LocalConfigurationBuilder();
var config = configBuilder.BuildJson("appsettings.json");

var rabbitMqSetupModel = new RabbitMqSetupModel(
    RabbitMqConfig.DefaultUri, RabbitMqConfig.DataCaptureExchange,
    RabbitMqConfig.DataCaptureQueue, RabbitMqConfig.DataCaptureRoutingKey
);
var resultFileInfo = new ResultFileInfo(
    config["dataPath"],
    config["fileExtension"]
);
using var rabbitMqService = new RabbitMQService(rabbitMqSetupModel, resultFileInfo);
rabbitMqService.Setup();

Console.WriteLine("RabbitMQ setup completed");

rabbitMqService.RegisterDataExchangeConsumer();

Console.WriteLine("DataExchangeConsumer is registered. Awaiting messages...");

Console.ReadKey();