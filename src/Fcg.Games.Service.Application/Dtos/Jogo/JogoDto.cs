namespace Fcg.Games.Service.Application.Dtos.Jogo;
public class JogoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Preco { get; set; }
    public bool Ativo { get; set; }
}
