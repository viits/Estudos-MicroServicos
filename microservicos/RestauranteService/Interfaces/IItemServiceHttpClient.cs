using RestauranteService.Dtos;

namespace RestauranteService.Interfaces
{
    public interface IItemServiceHttpClient
    {
        public void EnviaRestauranteParaItemService(RestauranteReadDto restauranteDTO);
    }
}
