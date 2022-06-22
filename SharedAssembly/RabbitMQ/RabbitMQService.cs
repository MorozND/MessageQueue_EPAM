using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedAssembly.Models;

namespace SharedAssembly.RabbitMQ
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routingKey;

        private readonly ResultFileInfo? _fileInfo;

        //private const int MAX_MESSAGE_SIZE = 128_000_000;
        private const int MAX_MESSAGE_SIZE = 5_000_000;

        public RabbitMQService(RabbitMqSetupModel? setupModel, ResultFileInfo? fileInfo = null)
        {
            ArgumentNullException.ThrowIfNull(setupModel);

            var factory = new ConnectionFactory();
            factory.Uri = setupModel.Uri;

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _exchangeName = setupModel.ExchangeName;
            _queueName = setupModel.QueueName;
            _routingKey = setupModel.RoutingKey;
            _fileInfo = fileInfo;
        }

        public void Setup()
        {
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(_queueName, false, false, false, null);
            _channel.QueueBind(_queueName, _exchangeName, _routingKey, null);
        }

        public async Task PublishFileAsync(string file)
        {
            var fileInfo = new FileInfo(file);

            var headers = new Dictionary<string, object>();
            var props = _channel.CreateBasicProperties();
            var sequence = 1;

            headers.Add(RabbitMqHeaders.FileName, Path.GetFileName(file));

            if (fileInfo.Length >= MAX_MESSAGE_SIZE)
            {
                using FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);

                byte[] buffer = new byte[MAX_MESSAGE_SIZE];
                while ((await fs.ReadAsync(buffer, 0, MAX_MESSAGE_SIZE)) != 0)
                {
                    headers[RabbitMqHeaders.Sequence] = sequence;

                    props.Headers = headers;
                    _channel.BasicPublish(_exchangeName, _routingKey, null, buffer);

                    sequence++;
                }
            }

            var content = await File.ReadAllBytesAsync(file);

            if (content is null || !content.Any())
                throw new ArgumentException("Invalid content");

            headers.Add(RabbitMqHeaders.Sequence, sequence);

            props.Headers = headers;
            _channel.BasicPublish(_exchangeName, _routingKey, props, content);
        }

        public void RegisterDataExchangeConsumer()
        {
            if (_fileInfo is null)
                throw new ArgumentException("Can't register DataExchangeConsumer without a path to store files into.");

            if (!Directory.Exists(_fileInfo.Path))
                Directory.CreateDirectory(_fileInfo.Path);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += DataExchangeConsumer_Received;

            _channel.BasicConsume(_queueName, false, consumer);
        }

        private void DataExchangeConsumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            if (_fileInfo is null)
                throw new InvalidDataException("Information about how to store files should be provided at consumer level");

            var content = e.Body.ToArray();

            File.WriteAllBytes(Path.Combine(_fileInfo.Path, $"{e.DeliveryTag}.{_fileInfo.Extension}"), content);

            _channel.BasicAck(e.DeliveryTag, false);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
