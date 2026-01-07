using Fcg.Games.Service.Application.Dtos.Jogo;

namespace Fcg.Games.Service.Application.Interfaces;
public interface IMetricaAppService
{
    Task<List<JogoDto>> ObterJogosMaisPopularesAsync(int top);
}
