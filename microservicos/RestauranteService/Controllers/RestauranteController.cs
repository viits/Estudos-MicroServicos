using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestauranteService.Data;
using RestauranteService.Dtos;
using RestauranteService.Interfaces;
using RestauranteService.Models;
using RestauranteService.RabbitMqClient;


namespace RestauranteService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RestauranteController : ControllerBase
{
    private readonly IRestauranteRepository _repository;
    private readonly IMapper _mapper;
    private IRabbitMqClient _rabbitMqClient;
    private IItemServiceHttpClient _itemServiceHttpClient;

    public RestauranteController(
        IRestauranteRepository repository,
        IMapper mapper, IItemServiceHttpClient itemServiceHttpClient, IRabbitMqClient rabbitMqClient)
    {
        _repository = repository;
        _mapper = mapper;
        _itemServiceHttpClient = itemServiceHttpClient;
        _rabbitMqClient = rabbitMqClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<RestauranteReadDto>> GetAllRestaurantes()
    {

        var restaurantes = _repository.GetAllRestaurantes();

        return Ok(_mapper.Map<IEnumerable<RestauranteReadDto>>(restaurantes));
    }

    [HttpGet("{id}", Name = "GetRestauranteById")]
    public ActionResult<RestauranteReadDto> GetRestauranteById(int id)
    {
        var restaurante = _repository.GetRestauranteById(id);
        if (restaurante != null)
        {
            return Ok(_mapper.Map<RestauranteReadDto>(restaurante));
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<RestauranteReadDto>> CreateRestaurante(RestauranteCreateDto restauranteCreateDto)
    {
        var restaurante = _mapper.Map<Restaurante>(restauranteCreateDto);
        _repository.CreateRestaurante(restaurante);
        _repository.SaveChanges();

        var restauranteReadDto = _mapper.Map<RestauranteReadDto>(restaurante);

        // _itemServiceHttpClient.EnviaRestauranteParaItemService(restauranteReadDto);
        _rabbitMqClient.PublicarRestaurante(restauranteReadDto);

        return CreatedAtRoute(nameof(GetRestauranteById), new { restauranteReadDto.Id }, restauranteReadDto);
    }
}
