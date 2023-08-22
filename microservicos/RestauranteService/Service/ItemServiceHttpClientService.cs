using RestauranteService.Dtos;
using RestauranteService.Interfaces;
using System.Text;
using System.Text.Json;

namespace RestauranteService.Service
{
    public class ItemServiceHttpClientService : IItemServiceHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public ItemServiceHttpClientService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

        }
        public async void EnviaRestauranteParaItemService(RestauranteReadDto restauranteDTO)
        {
            var conteudoHttp = new StringContent(
                    JsonSerializer.Serialize(restauranteDTO), Encoding.UTF8, "application/json"
                );
            await _httpClient.PostAsync(_configuration["ItemService"], conteudoHttp);
        }
    }
}
