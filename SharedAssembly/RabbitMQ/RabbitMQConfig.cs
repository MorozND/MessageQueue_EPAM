using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssembly.RabbitMQ
{
    public static class RabbitMQConfig
    {
        public static readonly Uri DefaultUri = new Uri("amqp://guest:guest@localhost:5672");
        public static readonly string DataCaptureExchange = "DataCaptureExchange";
        public static readonly string DataCaptureQueue = "DataCaptureQueue";
        public static readonly string DataCaptureRoutingKey = "data_capture";
    }
}
