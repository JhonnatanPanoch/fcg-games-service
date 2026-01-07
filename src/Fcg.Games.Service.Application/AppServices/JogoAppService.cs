using Fcg.Games.Service.Application.Dtos.Jogo;
using Fcg.Games.Service.Application.Interfaces;
using Fcg.Games.Service.Domain.Entities;
using Fcg.Games.Service.Domain.Exceptions;
using Fcg.Games.Service.Domain.Interfaces;
using Fcg.Games.Service.Infra.Elastic.Clients.Jogo.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fcg.Games.Service.Application.AppServices;

public class JogoAppService : IJogoAppService
{
    private readonly IRepository<JogoEntity> _jogoRepository;
    private readonly ILogger<JogoAppService> _logger;
    private readonly IJogoElastic _jogoElastic;

    public JogoAppService(
        IRepository<JogoEntity> jogoRepository, 
        ILogger<JogoAppService> logger, 
        IJogoElastic jogoElastic)
    {
        _jogoRepository = jogoRepository;
        _logger = logger;
        _jogoElastic = jogoElastic;
    }

    public async Task<IEnumerable<JogoDto>> ObterTodosAsync()
    {
        var dados = await _jogoRepository.ObterAsync();
        return dados.Select(x => new JogoDto
        {
            Id = x.Id,
            Nome = x.Nome,
            Descricao = x.Descricao,
            Preco = x.Preco,
            Ativo = x.Ativo,
        });
    }

    public async Task<JogoDto?> ObterPorIdAsync(Guid id)
    {
        var dado = await _jogoRepository.ObterPorIdAsync(id);

        if (dado == null)
            throw new NotFoundException();

        return new JogoDto()
        {
            Id = dado.Id,
            Nome = dado.Nome,
            Descricao = dado.Descricao,
            Preco = dado.Preco,
            Ativo = dado.Ativo,
        };
    }

    public async Task<JogoDto> CadastrarAsync(CadastrarJogoDto dto)
    {
        _logger.LogInformation("Tentativa de cadastro: {@Dto}", dto);

        var dbData = await _jogoRepository.ObterAsync(x => x.Nome == dto.Nome);
        if (dbData?.Count > 0)
        {
            _logger.LogWarning($"Um jogo com este nome: {dto.Nome} já está cadastrado no sistema.");
            throw new ConflictException("Um jogo com este nome já está cadastrado no sistema.");
        }

        var retorno = await _jogoRepository.AdicionarAsync(
            new JogoEntity(
                dto.Nome,
                dto.Descricao,
                dto.Preco));

        await _jogoElastic.IndexarJogoAsync(retorno);

        return new JogoDto()
        {
            Id = retorno.Id,
            Nome = retorno.Nome,
            Descricao = retorno.Descricao,
            Preco = retorno.Preco,
            Ativo = retorno.Ativo,
        };
    }

    public async Task AtualizarAsync(Guid id, AtualizarJogoDto dto)
    {
        _logger.LogInformation("Tentativa de cadastro: {@Dto}", dto);

        var dbData = await _jogoRepository.ObterPorIdAsync(id);
        if (dbData is null)
            throw new NotFoundException();

        var dbDataEmail = await _jogoRepository.ObterAsync(x => x.Id != id && x.Nome == dto.Nome);
        if (dbDataEmail?.Count != 0)
        {
            _logger.LogWarning($"Um jogo com este nome: {dto.Nome} já está cadastrado no sistema.");
            throw new ConflictException("Um jogo com este nome já está cadastrado no sistema.");
        }

        dbData.Nome = dto.Nome;
        dbData.Descricao = dto.Descricao;
        dbData.Preco = dto.Preco;

        await _jogoRepository.Atualizar(dbData);

        await _jogoElastic.IndexarJogoAsync(dbData);
    }

    public async Task RemoverAsync(Guid id)
    {
        var dbData = await _jogoRepository.ObterPorIdAsync(id);
        if (dbData is null)
            throw new NotFoundException();

        await _jogoRepository.Remover(dbData);
    }

    public async Task<List<JogoPrecoDto>> ConsultarPrecosAsync(List<Guid> ids)
    {
        var jogos = await _jogoRepository.ObterPorIdsAsync(ids);

        if (jogos is null || !jogos.Any())
            throw new NotFoundException();

        return jogos
            .Select(jogo => new JogoPrecoDto(
                jogo.Id,
                jogo.Nome,
                ObterPrecoJogo(jogo)))
            .ToList();
    }

    public async Task<IEnumerable<JogoDto>> BuscarJogosPorTermoAsync(string termoDeBusca)
    {
        var response = await _jogoElastic.BuscarJogosAsync(termoDeBusca);

        return response.Select(x => new JogoDto
        {
            Id = x.Id,
            Nome = x.Nome,
            Descricao = x.Descricao,
            Preco = x.Preco,
            Ativo = x.Ativo,
        });
    }

    private static decimal ObterPrecoJogo(JogoEntity jogo)
    {
        var agora = DateTime.Now;

        var promocaoAtiva = jogo.Promocoes?
            .FirstOrDefault(p => p.DataInicio <= agora && p.DataFim >= agora);

        return promocaoAtiva?.PrecoPromocional ?? jogo.Preco;
    }
}