namespace Fcg.Games.Service.Domain.Entities;

public class JogoEntity : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public bool Ativo { get; set; } = true;
    public virtual List<PromocaoEntity> Promocoes { get; set; } = [];

    public JogoEntity()
    {
    }

    public JogoEntity(
        string nome, 
        string descricao,
        decimal preco)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
    }
}