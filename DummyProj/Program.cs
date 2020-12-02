using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace DummyProj
{
    class Program
    {
        private const string QueueName = "logQueue";

        static void Main(string[] args)
        {
            // Queue code
            var connectionFactory = new ConnectionFactory { HostName = "localhost" };
            using IConnection connection = connectionFactory.CreateConnection();
            using IModel channel = connection.CreateModel();
            QueueDeclare(channel);
            Console.WriteLine("Receiving messages, press return to exit");
            ReceiveMessages(channel);
            Console.ReadLine();
        }

        private static void QueueDeclare(IModel channel)
        {
            // Queue code
            channel.QueueDeclare(QueueName, false, false, false, null);
        }

        private static void ReceiveMessages(IModel channel)
        {
            // Queue code
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Message :" + message);
            };

            channel.BasicConsume(QueueName, true, consumer);
        }
    }
}
