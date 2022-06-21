
using DataCaptureService.Services;
using Microsoft.Extensions.Configuration;
using SharedAssembly.RabbitMQ;

Console.WriteLine("DataCaptureService started");

var config = GetConfiguration("appsettings.json");

var dataPath = config["dataPath"];
var fileExtention = Environment.GetCommandLineArgs().FirstOrDefault() ?? "jpg";

Console.WriteLine($"Target data folder: {dataPath}");
Console.WriteLine($"Looking for {fileExtention} files...");

using var rabbitMqService = new RabbitMQService(RabbitMQConfig.DefaultUri);
var fileService = new FileService(dataPath);
var iterationPause = TimeSpan.FromSeconds(int.Parse(config["iterationPauseInSeconds"]));

while (true)
{
    var files = fileService.GetNewFiles(fileExtention);

    if (files.Any())
    {
        foreach (var file in files)
        {
            throw new NotImplementedException();
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
