using System.Text.Json;
using AutoMapper;
using ItemService.Data;
using ItemService.Models;

namespace ItemService.EventProcessor
{
    public class ProcessoEventoService : IProcessoEvento
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;
        public ProcessoEventoService(IMapper mapper, IServiceScopeFactory scopeFactory)
        {
            _mapper = mapper;
            _scopeFactory = scopeFactory;

        }
        public void Processa(string mensagem)
        {
            using var scope = _scopeFactory.CreateScope();
            var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
            var restauranteReadDTO = JsonSerializer.Deserialize<Restaurante>(mensagem);

            var restaurante = _mapper.Map<Restaurante>(restauranteReadDTO);
            if(!itemRepository.ExisteRestauranteExterno(restaurante.Id)){
                itemRepository.CreateRestaurante(restaurante);
                itemRepository.SaveChanges();
            }
        }

    }
}