
using RabbitMQ.Client;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

var connection = factory.CreateConnection();

Console.WriteLine("Connection opened");

var channel = connection.CreateModel();

Console.WriteLine("Channel created");

channel.ExchangeDeclare("DataCaptureExchange", ExchangeType.Direct);
channel.QueueDeclare("DataCaptureQueue", false, false, false, null);
channel.QueueBind("DataCaptureQueue", "DataCaptureExchange", "data_capture", null);

Console.WriteLine("Queue created");

Console.ReadKey();

connection.Close();