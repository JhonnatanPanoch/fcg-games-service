using Fcg.Games.Service.Application.Dtos.Jogo.Validations;
using Fcg.Games.Service.Application.Dtos.Promocao.Validations;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Fcg.Games.Service.Api.ApiConfigurations;
public static class RegisterValidations
{
    public static IServiceCollection AddAbstractValidations(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CadastrarJogoDtoValidator).Assembly);
        services.AddValidatorsFromAssembly(typeof(AtualizarJogoDtoValidator).Assembly);

        services.AddValidatorsFromAssembly(typeof(CadastrarPromocaoDtoValidator).Assembly);
        
        services.AddFluentValidationAutoValidation(options =>
        {
            options.OverrideDefaultResultFactoryWith<CustomValidatorResult>();
        });
        
        return services;
    }
}