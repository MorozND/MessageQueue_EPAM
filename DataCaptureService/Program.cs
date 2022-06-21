using DataCaptureService.Services;
using Microsoft.Extensions.Configuration;
using SharedAssembly.Models;
using SharedAssembly.RabbitMQ;

Console.WriteLine("DataCaptureService started");

var config = GetConfiguration("appsettings.json");

var dataPath = config["dataPath"];
var fileExtention = config["fileExtension"];

Console.WriteLine($"Looking for {fileExtention} files...");
Console.WriteLine($"Target data folder: {dataPath}");

var rabbitMqSetupModel = new RabbitMqSetupModel(
    RabbitMQConfig.DefaultUri, RabbitMQConfig.DataCaptureExchange,
    RabbitMQConfig.DataCaptureQueue, RabbitMQConfig.DataCaptureRoutingKey
);
using var rabbitMqService = new RabbitMQService(rabbitMqSetupModel);
var fileService = new FileService(dataPath);
var iterationPause = TimeSpan.FromSeconds(int.Parse(config["iterationPauseInSeconds"]));

while (true)
{
    var files = fileService.GetNewFiles(fileExtention);

    if (files.Any())
    {
        foreach (var file in files)
        {
            var fileContent = await File.ReadAllBytesAsync(file);
            rabbitMqService.PublishMessage(fileContent);
        }
    }

    Thread.Sleep(iterationPause);
}

IConfiguration GetConfiguration(string fileName)
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile(fileName);

    return configuration.Build();
}
