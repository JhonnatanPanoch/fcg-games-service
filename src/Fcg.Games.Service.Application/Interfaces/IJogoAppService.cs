using Fcg.Games.Service.Application.Dtos.Jogo;

namespace Fcg.Games.Service.Application.Interfaces;

public interface IJogoAppService
{
    Task<IEnumerable<JogoDto>> ObterTodosAsync();
    Task<JogoDto?> ObterPorIdAsync(Guid id);
    Task<JogoDto> CadastrarAsync(CadastrarJogoDto jogo);
    Task AtualizarAsync(Guid id, AtualizarJogoDto jogo);
    Task RemoverAsync(Guid id);
    Task<List<JogoPrecoDto>> ConsultarPrecosAsync(List<Guid> ids);
    Task<IEnumerable<JogoDto>> BuscarJogosPorTermoAsync(string termoDeBusca);
}