using System;
using System.Text;
using System.Text.Json;
using Lab_3_4.Events;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Lab_3_4.Services
{
    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqService(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeRabbitMq();
        }

        private void InitializeRabbitMq()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Объявление exchange
                _channel.ExchangeDeclare("cars_events_exchange", ExchangeType.Fanout);

                // Объявление очереди
                _channel.QueueDeclare(queue: "cars_events_queue",
                                      durable: false,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);

                // Привязка очереди к exchange
                _channel.QueueBind(queue: "cars_events_queue",
                                   exchange: "cars_events_exchange",
                                   routingKey: string.Empty,
                                   arguments: null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения к RabbitMQ: {ex.Message}");
            }
        }

        public void Publish(CarEvent carEvent)
        {
            if (_channel == null || !_channel.IsOpen)
            {
                Console.WriteLine("Канал RabbitMQ не открыт.");
                return;
            }

            var json = JsonSerializer.Serialize(carEvent);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: "cars_events_exchange",
                                  routingKey: "",
                                  basicProperties: null,
                                  body: body);

            Console.WriteLine($"Событие отправлено: {json}");
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}