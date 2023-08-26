using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RestauranteService.Dtos;

namespace RestauranteService.RabbitMqClient
{
    public class RabbitMqClientService : IRabbitMqClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private IModel _channel;

        public RabbitMqClientService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new ConnectionFactory(){HostName = _configuration["RabbitMqHost"], Port =Int32.Parse(_configuration["RabbitMqPort"])}.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        }
        public void PublicarRestaurante(RestauranteReadDto restauranteDTO)
        {
            string mensagem = JsonSerializer.Serialize(restauranteDTO);
            var body = Encoding.UTF8.GetBytes(mensagem);
            _channel.BasicPublish(exchange:"trigger", routingKey:"", basicProperties:null, body: body);
        }
    }
}