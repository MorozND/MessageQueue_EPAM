using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedAssembly.Models;
using System.Text;

namespace SharedAssembly.RabbitMQ
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routingKey;

        private readonly string? _dataPath;

        //private const int MAX_MESSAGE_SIZE = 128_000_000;
        private const int MAX_MESSAGE_SIZE = 50_000_000;

        public RabbitMQService(RabbitMqSetupModel? setupModel, string? dataPath = null)
        {
            ArgumentNullException.ThrowIfNull(setupModel);

            var factory = new ConnectionFactory();
            factory.Uri = setupModel.Uri;

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _exchangeName = setupModel.ExchangeName;
            _queueName = setupModel.QueueName;
            _routingKey = setupModel.RoutingKey;

            _dataPath = dataPath;
        }

        public void Setup()
        {
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(_queueName, false, false, false, null);
            _channel.QueueBind(_queueName, _exchangeName, _routingKey, null);
        }

        public async Task PublishFileAsync(string file)
        {
            var headers = new Dictionary<string, object>();
            var props = _channel.CreateBasicProperties();
            var sequence = 1;

            headers.Add(RabbitMqHeaders.FileName, Path.GetFileName(file));

            using FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);

            byte[] buffer = new byte[MAX_MESSAGE_SIZE];
            while ((await fs.ReadAsync(buffer, 0, MAX_MESSAGE_SIZE)) != 0)
            {
                headers[RabbitMqHeaders.Sequence] = sequence;

                props.Headers = headers;
                _channel.BasicPublish(_exchangeName, _routingKey, props, buffer);
                 
                sequence++;
            }
        }

        public void RegisterDataExchangeConsumer()
        {
            if (string.IsNullOrWhiteSpace(_dataPath))
                throw new InvalidDataException("Can't register DataExcahngeConsumer without data path");

            if (!Directory.Exists(_dataPath))
                Directory.CreateDirectory(_dataPath);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += DataExchangeConsumer_Received;

            _channel.BasicConsume(_queueName, false, consumer);
        }

        private void DataExchangeConsumer_Received(object? sender, BasicDeliverEventArgs eventArgs)
        {
            if (string.IsNullOrWhiteSpace(_dataPath))
                throw new InvalidDataException("Data path is not specified");

            var content = eventArgs.Body.ToArray();

            var messageMetadata = GetFileMessageMetadata(eventArgs);

            var fullFilePath = Path.Combine(_dataPath, messageMetadata.FileName);

            using FileStream fs = File.Open(fullFilePath, FileMode.Append, FileAccess.Write);
            fs.Write(content, 0, content.Length);

            Console.WriteLine($"\nPROCESSED\tFile: {messageMetadata.FileName}; Sequence: {messageMetadata.Sequence};");

            _channel.BasicAck(eventArgs.DeliveryTag, false);
        }

        private FileMessageMetadata GetFileMessageMetadata(BasicDeliverEventArgs e)
        {
            if (!e.BasicProperties.Headers.TryGetValue(RabbitMqHeaders.FileName, out var fileNameHeader) || fileNameHeader is null)
                throw new InvalidDataException("Can't get FileName from message");

            if (!e.BasicProperties.Headers.TryGetValue(RabbitMqHeaders.Sequence, out var sequenceHeader) || sequenceHeader is null)
                throw new InvalidDataException("Can't get Sequence from message");

            return new FileMessageMetadata(
                Encoding.UTF8.GetString((byte[])fileNameHeader),
                sequenceHeader as int?
            );
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
