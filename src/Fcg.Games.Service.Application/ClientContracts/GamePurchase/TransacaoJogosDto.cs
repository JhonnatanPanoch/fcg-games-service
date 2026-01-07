namespace Fcg.Games.Service.Application.ClientContracts.GamePurchase;
public class TransacaoJogosDto
{
    public Guid JogoId { get; set; }
    public DateTime DataAquisicao { get; set; }
    public decimal PrecoPago { get; set; }
}
