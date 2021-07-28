using System;
using System.Text;
using System.Threading.Tasks;
using LogServer.LoggerRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static Constants.Constants;

namespace LogServer
{
    public class Program
    {
        private const string QueueName = "logQueue";
        
        public static void Main(string[] args)
        {
            ReceiveLogs();
            CreateHostBuilder(args).Build().Run();
        }

        private static void ReceiveLogs()
        {
            IModel channel = new ConnectionFactory { HostName = "localhost" }.CreateConnection().CreateModel();

            channel.QueueDeclare(QueueName, false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            Task.Run(() =>
            {
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        LogRepository.Instance().AddLog(message);
                        Console.WriteLine(message);
                    };

                channel.BasicConsume(QueueName, true, consumer);
            });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(String.Format(UriToFormat, localhostHttps, LogServerPort));
                });
    }
}
