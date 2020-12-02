using RabbitMQ.Client;
using Repository.ServiceInterfaces;
using System.Text;

namespace Repository.Services
{
    public class LogSenderService : ILogSenderService
    {
        private const string _queueName = "logQueue";
        private IModel _channel;

        public LogSenderService()
        {
            var connectionFactory = new ConnectionFactory { HostName = "localhost" };
            IConnection connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            QueueDeclare();
        }

        private void QueueDeclare()
        {
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void SendMessages(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var body = Encoding.UTF8.GetBytes(message);
                _channel.BasicPublish(exchange: string.Empty, routingKey: _queueName, basicProperties: null, body: body);
            }
        }
    }
}
