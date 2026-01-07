using Fcg.Games.Service.Domain.Entities;
using FluentAssertions;

namespace Fcg.Games.Service.Test.Tests.Jogos;

public class JogoEntityUnitTests
{
    [Fact]
    public void ConstrutorPadrao_DeveInicializarPropriedadesComValoresPadrao()
    {
        // Act
        var jogo = new JogoEntity();

        // Assert
        jogo.Nome.Should().BeEmpty();
        jogo.Descricao.Should().BeEmpty();
        jogo.Preco.Should().Be(0m);
        jogo.Ativo.Should().BeTrue();
        jogo.Promocoes.Should().NotBeNull();
        jogo.Promocoes.Should().BeEmpty();
    }

    [Fact]
    public void ConstrutorParametrizado_DeveAtribuirPropriedadesCorretamente()
    {
        // Arrange
        var nome = "Jogo Teste";
        var descricao = "Descrição do jogo";
        var preco = 49.99m;

        // Act
        var jogo = new JogoEntity(nome, descricao, preco);

        // Assert
        jogo.Nome.Should().Be(nome);
        jogo.Descricao.Should().Be(descricao);
        jogo.Preco.Should().Be(preco);
        jogo.Ativo.Should().BeTrue();
        jogo.Promocoes.Should().NotBeNull();
        jogo.Promocoes.Should().BeEmpty();
    }

    [Fact]
    public void Propriedades_DevemSerAtribuidasCorretamente()
    {
        // Arrange
        var nome = "Novo Jogo";
        var descricao = "Nova descrição";
        var preco = 99.99m;
        var ativo = false;
        var promocoes = new List<PromocaoEntity>();

        // Act
        var jogo = new JogoEntity
        {
            Nome = nome,
            Descricao = descricao,
            Preco = preco,
            Ativo = ativo,
            Promocoes = promocoes
        };

        // Assert
        jogo.Nome.Should().Be(nome);
        jogo.Descricao.Should().Be(descricao);
        jogo.Preco.Should().Be(preco);
        jogo.Ativo.Should().BeFalse();
        jogo.Promocoes.Should().BeSameAs(promocoes);
    }
}
