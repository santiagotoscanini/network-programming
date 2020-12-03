using RabbitMQ.Client;
using System.Text;
using static Constants.Constants;
using System;

namespace Repository.Services
{
    public class LogSenderService
    {
        private const string _queueName = "logQueue";
        private IModel _channel;

        public LogSenderService()
        {
            _channel = new ConnectionFactory { HostName = "localhost" }.CreateConnection().CreateModel();
            QueueDeclare(_channel);
        }

        public void SendMessages(string logToSend)
        {
            logToSend = string.Format(InfoLogFormat, DateTime.Today.ToString("D"), "Repository Server", logToSend);

            PublishMessage(logToSend, _channel);
        }

        private void QueueDeclare(IModel channel)
        {
            channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void PublishMessage(string message, IModel channel)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: string.Empty, routingKey: _queueName, basicProperties: null, body: body);
            }
        }
    }
}
