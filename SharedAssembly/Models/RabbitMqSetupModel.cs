namespace SharedAssembly.Models
{
    public class RabbitMqSetupModel
    {
        public Uri Uri { get; private set; }
        public string ExchangeName { get; private set; }
        public string QueueName { get; private set; }
        public string RoutingKey { get; private set; }

        public RabbitMqSetupModel(Uri? uri, string? exchangeName, string? queueName, string? routingKey)
        {
            ArgumentNullException.ThrowIfNull(uri);
            ArgumentNullException.ThrowIfNull(exchangeName);
            ArgumentNullException.ThrowIfNull(queueName);
            ArgumentNullException.ThrowIfNull(routingKey);

            Uri = uri;
            ExchangeName = exchangeName;
            QueueName = queueName;
            RoutingKey = routingKey;
        }
    }
}
