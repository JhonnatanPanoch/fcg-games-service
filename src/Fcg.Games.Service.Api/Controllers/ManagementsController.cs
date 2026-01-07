using Fcg.Games.Service.Domain.Entities;
using Fcg.Games.Service.Domain.Interfaces;
using Fcg.Games.Service.Infra.Elastic.Clients.Jogo.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Games.Service.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ManagementsController : MainController
{
    private readonly IRepository<JogoEntity> _jogoRepository;
    private readonly IJogoElastic _jogoElastic;

    public ManagementsController(IRepository<JogoEntity> jogoRepository, IJogoElastic jogoElastic)
    {
        _jogoRepository = jogoRepository;
        _jogoElastic = jogoElastic;
    }

    [HttpPost]
    public async Task<IActionResult> AtualizarElastic()
    {
        var jogos = await _jogoRepository.ObterAsync();

        foreach (var jogo in jogos)
        {
            await _jogoElastic.IndexarJogoAsync(new JogoEntity()
            {
                Id = jogo.Id,
                Nome = jogo.Nome,
                Descricao = jogo.Descricao,
                Preco = jogo.Preco,
                Ativo = jogo.Ativo
            });
        }

        return Ok();
    }
}