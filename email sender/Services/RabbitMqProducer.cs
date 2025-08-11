using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace email_sender.Services
{
    public class RabbitMqProducer
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqProducer(IConfiguration configuration)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };
        }

        public async Task SendMessageAsync(object message)
        {
            const int maxRetries = 3;
            const int delayMs = 1000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Create async connection and channel
                    using var connection = await _factory.CreateConnectionAsync();
                    using var channel = await connection.CreateChannelAsync();

                    // Declare the queue (idempotent)
                    await channel.QueueDeclareAsync(queue: "email_tasks",
                                                  durable: false,
                                                  exclusive: false,
                                                  autoDelete: false,
                                                  arguments: null);

                    // Serialize the message to JSON and get bytes
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                    // Publish the message to the queue
                    await channel.BasicPublishAsync(exchange: "",
                                                  routingKey: "email_tasks",
                                                  body: body);
                    return; // Success, exit the retry loop
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}. Retrying in {delayMs}ms...");
                    await Task.Delay(delayMs);
                }
            }
        }
    }
}
