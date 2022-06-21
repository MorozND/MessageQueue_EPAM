using RabbitMQ.Client;
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

        public RabbitMQService(RabbitMqSetupModel setupModel)
        {
            var factory = new ConnectionFactory();
            factory.Uri = setupModel.Uri;

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _exchangeName = setupModel.ExchangeName;
            _queueName = setupModel.QueueName;
            _routingKey = setupModel.RoutingKey;
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

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
