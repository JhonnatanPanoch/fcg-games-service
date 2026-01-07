namespace Fcg.Games.Service.Application.Dtos.Promocao;

public class CadastrarPromocaoDto
{
    public Guid IdJogo { get; set; }
    public decimal PrecoPromocional { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime Final { get; set; }
}