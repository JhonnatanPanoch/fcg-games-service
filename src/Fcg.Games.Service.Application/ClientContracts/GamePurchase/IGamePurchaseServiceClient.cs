
namespace Fcg.Games.Service.Application.ClientContracts.GamePurchase;
public interface IGamePurchaseServiceClient
{
    Task<List<TransacaoJogosCompraDto>> ConsultarTransacaoAsync(Guid idUsuario);
    Task<List<JogoMaisVendidoDto>> ObterMaisVendidosAsync(int top);
}
