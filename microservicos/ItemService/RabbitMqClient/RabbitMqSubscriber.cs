using System.Text;
using ItemService.EventProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ItemService.RabbitMqClient
{
    public class RabbitMqSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly string _nomeFila;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IProcessoEvento _processoEvento;
        public RabbitMqSubscriber(IConfiguration configuration, IProcessoEvento processoEvento)
        {
            _configuration = configuration;
            _connection = new ConnectionFactory() 
                { HostName = _configuration["RabbitMQHost"], Port = Int32.Parse(_configuration["RabbitMQPort"]) }.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _nomeFila = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _nomeFila, exchange: "trigger", routingKey: "");
            _processoEvento = processoEvento;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumidor = new EventingBasicConsumer(_channel);
            consumidor.Received += (ModuleHandle, ea) =>
            {
                ReadOnlyMemory<byte> body = ea.Body;
                var mensagem = Encoding.UTF8.GetString(body.ToArray());
                _processoEvento.Processa(mensagem);

            };
            _channel.BasicConsume(queue: _nomeFila, autoAck: true, consumer: consumidor);
            return Task.CompletedTask;
        }
    }
}