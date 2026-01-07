namespace Fcg.Games.Service.Domain.Entities;
public class PromocaoEntity : EntityBase
{
    public Guid JogoId { get; set; }
    public virtual JogoEntity Jogo { get; set; } = null!;

    public decimal PrecoPromocional { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public bool Ativa => DateTime.UtcNow >= DataInicio && DateTime.UtcNow <= DataFim;
}
