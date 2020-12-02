using LogServer.LoggerRepositoryInterface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogServer.Services
{
    public class ReceiverService
    {
        private const string QueueName = "logQueue";
        private ILogRepository _logRepository;

        public ReceiverService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
            var connectionFactory = new ConnectionFactory { HostName = "localhost" };
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            QueueDeclare(channel);
            ReceiveMessages(channel);
        }

        private void QueueDeclare(IModel channel)
        {
            channel.QueueDeclare(QueueName, false, false, false, null);
        }

        private void ReceiveMessages(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logRepository.AddLog(message);
                Console.WriteLine(message);
            };

            channel.BasicConsume(QueueName, true, consumer);
        }

    }
}
