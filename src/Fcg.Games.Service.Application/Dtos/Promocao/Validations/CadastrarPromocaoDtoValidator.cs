using FluentValidation;

namespace Fcg.Games.Service.Application.Dtos.Promocao.Validations;
public class CadastrarPromocaoDtoValidator : AbstractValidator<CadastrarPromocaoDto>
{
    public CadastrarPromocaoDtoValidator()
    {
        RuleFor(x => x.IdJogo)
            .NotEmpty().WithMessage("O campo 'IdJogo' é obrigatório.");

        RuleFor(x => x.PrecoPromocional)
            .GreaterThan(0).WithMessage("O 'Preço Promocional' deve ser maior que zero.");

        RuleFor(x => x.Inicio)
            .Must(data => data > DateTime.MinValue).WithMessage("A data de início é inválida.")
            .LessThanOrEqualTo(x => x.Final).WithMessage("A data de início deve ser menor que a data final.");

        RuleFor(x => x.Final)
            .Must(data => data > DateTime.MinValue).WithMessage("A data final é inválida.");
    }
}
