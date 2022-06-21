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

        private readonly string? _consumerPath;

        public RabbitMQService(RabbitMqSetupModel setupModel, string? consumerPath = null)
        {
            var factory = new ConnectionFactory();
            factory.Uri = setupModel.Uri;

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _exchangeName = setupModel.ExchangeName;
            _queueName = setupModel.QueueName;
            _routingKey = setupModel.RoutingKey;
            _consumerPath = consumerPath;
        }

        public void Setup()
        {
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(_queueName, false, false, false, null);
            _channel.QueueBind(_queueName, _exchangeName, _routingKey, null);
        }

        public void PublishMessage(byte[] content)
        {
            if (content is null || !content.Any())
                throw new ArgumentException("Invalid content");

            _channel.BasicPublish(_exchangeName, _routingKey, null, content);
        }

        public void RegisterDataExchangeConsumer()
        {
            if (string.IsNullOrWhiteSpace(_consumerPath))
                throw new ArgumentException("Can't register DataExchangeConsumer without a path to store files into.");

            if (!Directory.Exists(_consumerPath))
                Directory.CreateDirectory(_consumerPath);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += DataExchangeConsumer_Received;

            _channel.BasicConsume(_queueName, false, consumer);
        }

        private void DataExchangeConsumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var content = e.Body.ToArray();

            File.WriteAllBytes(Path.Combine(_consumerPath!, $"{e.DeliveryTag}.jpg"), content);

            _channel.BasicAck(e.DeliveryTag, false);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
