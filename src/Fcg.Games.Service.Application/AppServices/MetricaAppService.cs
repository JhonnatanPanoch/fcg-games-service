using Fcg.Games.Service.Application.ClientContracts.GamePurchase;
using Fcg.Games.Service.Application.Dtos.Jogo;
using Fcg.Games.Service.Application.Interfaces;
using Fcg.Games.Service.Domain.Entities;
using Fcg.Games.Service.Domain.Interfaces;

namespace Fcg.Games.Service.Application.AppServices;
public class MetricaAppService : IMetricaAppService
{
    private readonly IGamePurchaseServiceClient _purchasingServiceClient;
    private readonly IRepository<JogoEntity> _jogoRepository;

    public MetricaAppService(
        IGamePurchaseServiceClient purchasingServiceClient,
        IRepository<JogoEntity> jogoRepository)
    {
        _purchasingServiceClient = purchasingServiceClient;
        _jogoRepository = jogoRepository;
    }

    public async Task<List<JogoDto>> ObterJogosMaisPopularesAsync(int top)
    {
        var maisVendidosIds = await _purchasingServiceClient.ObterMaisVendidosAsync(top);
        if (maisVendidosIds is null || !maisVendidosIds.Any())
            return [];

        var jogoIds = maisVendidosIds.Select(j => j.JogoId).ToList();
        var jogos = await _jogoRepository.ObterPorIdsAsync(jogoIds);

        var jogosDict = jogos.ToDictionary(j => j.Id);
        var jogosOrdenados = maisVendidosIds
            .Select(idInfo => jogosDict.GetValueOrDefault(idInfo.JogoId))
            .Where(j => j != null)
            .Select(j => new JogoDto()
            {
                Id = j.Id,
                Nome = j.Nome,
                Descricao = j.Descricao,
                Preco = j.Preco,
                Ativo = j.Ativo
            }).ToList();

        return jogosOrdenados;
    }
}
