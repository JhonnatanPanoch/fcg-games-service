using Fcg.Games.Service.Domain.Entities;
using FluentAssertions;

namespace Fcg.Games.Service.Test.Tests.Promocao;

public class PromocaoEntityUnitTests
{
    [Fact]
    public void Ativa_DeveRetornarTrue_QuandoDataAtualEntreInicioEFim()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var promocao = new PromocaoEntity
        {
            DataInicio = now.AddMinutes(-10),
            DataFim = now.AddMinutes(10)
        };

        // Act & Assert
        promocao.Ativa.Should().BeTrue();
    }

    [Fact]
    public void Ativa_DeveRetornarFalse_QuandoDataAtualAntesDeInicio()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var promocao = new PromocaoEntity
        {
            DataInicio = now.AddMinutes(1),
            DataFim = now.AddMinutes(10)
        };

        // Act & Assert
        promocao.Ativa.Should().BeFalse();
    }

    [Fact]
    public void Ativa_DeveRetornarFalse_QuandoDataAtualDepoisDeFim()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var promocao = new PromocaoEntity
        {
            DataInicio = now.AddMinutes(-10),
            DataFim = now.AddMinutes(-1)
        };

        // Act & Assert
        promocao.Ativa.Should().BeFalse();
    }

    [Fact]
    public void PropriedadesDevemSerAtribuidasCorretamente()
    {
        // Arrange
        var jogoId = Guid.NewGuid();
        var jogo = new JogoEntity();
        var precoPromocional = 9.99m;
        var dataInicio = DateTime.UtcNow;
        var dataFim = DateTime.UtcNow.AddDays(1);

        // Act
        var promocao = new PromocaoEntity
        {
            JogoId = jogoId,
            Jogo = jogo,
            PrecoPromocional = precoPromocional,
            DataInicio = dataInicio,
            DataFim = dataFim
        };

        // Assert
        promocao.JogoId.Should().Be(jogoId);
        promocao.Jogo.Should().Be(jogo);
        promocao.PrecoPromocional.Should().Be(precoPromocional);
        promocao.DataInicio.Should().Be(dataInicio);
        promocao.DataFim.Should().Be(dataFim);
    }
}
