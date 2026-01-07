using FluentValidation;

namespace Fcg.Games.Service.Application.Dtos.Jogo.Validations;
public class AtualizarJogoDtoValidator : AbstractValidator<AtualizarJogoDto>
{
    public AtualizarJogoDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do jogo é obrigatório.")
            .MinimumLength(3).WithMessage("O nome do jogo deve ter pelo menos 3 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição do jogo é obrigatória.")
            .MinimumLength(10).WithMessage("A descrição do jogo deve ter pelo menos 10 caracteres.");

        RuleFor(x => x.Preco)
            .GreaterThanOrEqualTo(0).WithMessage("O preço do jogo deve ser maior ou igual a zero.");
    }
}
