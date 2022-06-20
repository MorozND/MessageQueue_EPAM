using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssembly.RabbitMQ
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;

        public RabbitMQService(Uri connectionUri)
        {
            var factory = new ConnectionFactory();
            factory.Uri = connectionUri;

            _connection = factory.CreateConnection();
        }

        public void SetupExchangeAndQueue(string exchangeName, string queueName, string routingKey)
        {
            var channel = _connection.CreateModel();

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);

            channel.Close();
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
