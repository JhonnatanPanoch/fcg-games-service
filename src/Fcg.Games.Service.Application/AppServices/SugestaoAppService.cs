using Fcg.Games.Service.Application.ClientContracts.GamePurchase;
using Fcg.Games.Service.Application.Dtos.Jogo;
using Fcg.Games.Service.Application.Interfaces;
using Fcg.Games.Service.Domain.Entities;
using Fcg.Games.Service.Domain.Interfaces;
using Fcg.Games.Service.Infra.Elastic.Clients.Jogo.Interfaces;

namespace Fcg.Games.Service.Application.AppServices;
public class SugestaoAppService : ISugestaoAppService
{
    private readonly IJogoElastic _elasticClient;
    private readonly IGamePurchaseServiceClient _purchasingServiceClient;
    private readonly IRepository<JogoEntity> _jogoRepository;
    private readonly IMetricaAppService _metricasAppService;

    public SugestaoAppService(
        IGamePurchaseServiceClient purchasingServiceClient,
        IRepository<JogoEntity> jogoRepository,
        IJogoElastic elasticClient,
        IMetricaAppService metricasAppService)
    {
        _purchasingServiceClient = purchasingServiceClient;
        _jogoRepository = jogoRepository;
        _elasticClient = elasticClient;
        _metricasAppService = metricasAppService;
    }

    public async Task<List<JogoDto>> ObterSugestoesParaUsuarioAsync(Guid usuarioId)
    {
        var transacoesUsuario = await _purchasingServiceClient.ConsultarTransacaoAsync(usuarioId);
        var jogosPossuidosIds = transacoesUsuario.SelectMany(t => t.Jogos.Select(x => x.JogoId)).ToList();

        if (!jogosPossuidosIds.Any())
            return await _metricasAppService.ObterJogosMaisPopularesAsync(5);

        var jogosPossuidos = await _jogoRepository.ObterPorIdsAsync(jogosPossuidosIds);

        var response = await _elasticClient.BuscarSugestaodeJogosAsync(jogosPossuidosIds);

        return response.Select(j => new JogoDto()
        {
            Id = j.Id,
            Nome = j.Nome,
            Descricao = j.Descricao,
            Preco = j.Preco,
            Ativo = j.Ativo
        }).ToList();
    }
}
