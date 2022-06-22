using DataCaptureService.Services;
using SharedAssembly;
using SharedAssembly.Models;
using SharedAssembly.RabbitMQ;

Console.WriteLine("DataCaptureService started");

var configBuilder = new LocalConfigurationBuilder();
var config = configBuilder.BuildJson("appsettings.json");

var dataPath = config["dataPath"];
var fileExtention = config["fileExtension"];

Console.WriteLine($"Target data folder: {dataPath}");

var rabbitMqSetupModel = new RabbitMqSetupModel(
    RabbitMqConfig.DefaultUri, RabbitMqConfig.DataCaptureExchange,
    RabbitMqConfig.DataCaptureQueue, RabbitMqConfig.DataCaptureRoutingKey
);
using var rabbitMqService = new RabbitMQService(rabbitMqSetupModel);
var fileService = new FileService(dataPath);
var iterationPause = TimeSpan.FromSeconds(int.Parse(config["iterationPauseInSeconds"]));

Console.WriteLine($"Looking for {fileExtention} files...");
while (true)
{
    var files = fileService.GetNewFiles(fileExtention);

    if (files.Any())
    {
        Console.WriteLine($"\nFound {files.Length} new files.");

        foreach (var file in files)
        {
            Console.WriteLine($"Processing {Path.GetFileName(file)} ...");

            await rabbitMqService.PublishFileAsync(file);

            fileService.SetProcessedFile(file);
        }
    }
    else
    {
        Console.WriteLine("\nNo new files found");
    }

    Thread.Sleep(iterationPause);
}
