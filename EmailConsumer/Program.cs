using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Email Consumer Service Starting...");
        Console.WriteLine("Listening for email tasks from RabbitMQ");
        Console.WriteLine("Press [Enter] to exit.\n");

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        try
        {
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // Declare the same queue as the producer
            await channel.QueueDeclareAsync(
                queue: "email_tasks",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine("Connected to RabbitMQ. Waiting for messages...\n");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    // Deserialize the email task
                    var emailTask = JsonSerializer.Deserialize<EmailTask>(message);

                    if (emailTask != null)
                    {
                        // Format the output as required: "2023.04.10 18:30 test@example.com код: 1234"
                        var timestamp = emailTask.Timestamp.ToString("yyyy.MM.dd HH:mm");
                        Console.WriteLine($"{timestamp} {emailTask.Email} Code: {emailTask.Code}");
                    }

                    // Acknowledge the message
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    // Reject the message and don't requeue it
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await channel.BasicConsumeAsync(
                queue: "email_tasks",
                autoAck: false, // Manual acknowledgment for reliability
                consumer: consumer);

            // Keep the application running
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to RabbitMQ: {ex.Message}");
            Console.WriteLine("Make sure RabbitMQ is running on localhost:5672");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}

// Same model as in the API
public class EmailTask
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}