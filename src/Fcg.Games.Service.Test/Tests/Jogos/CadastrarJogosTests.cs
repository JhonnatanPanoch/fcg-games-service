using Fcg.Games.Service.Application.Dtos.Jogo;
using Fcg.Games.Service.Domain.Exceptions.Responses;
using Fcg.Games.Service.Test.Configurations;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Fcg.Games.Service.Test.Tests.Jogos;

public class CadastrarJogosTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CadastrarJogosTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenCache.GetAdminToken(factory.Services));
    }

    [Fact(DisplayName = "Deve cadastrar um novo jogo")]
    [Trait("Api", "JogosController")]
    public async Task Quando_DadosValidos_Entao_CadastraJogoERetornaCreated()
    {
        // Arrange
        var dto = new CadastrarJogoDto
        {
            Nome = "Jogo Teste",
            Descricao = "Descrição do jogo teste",
            Preco = 99.99m
        };
        var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/jogos", content);
        var jogo = JsonConvert.DeserializeObject<JogoDto>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        jogo.Should().NotBeNull();
        jogo.Nome.Should().Be(dto.Nome);
        jogo.Descricao.Should().Be(dto.Descricao);
        jogo.Preco.Should().Be(dto.Preco);
        jogo.Ativo.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve retornar Conflict ao cadastrar jogo com nome já existente")]
    [Trait("Api", "JogosController")]
    public async Task Quando_NomeJaExiste_DeveRetornarConflict()
    {
        // Arrange
        var dto = new CadastrarJogoDto
        {
            Nome = $"Jogo 1",
            Descricao = "Descrição do jogo duplicado",
            Preco = 59.99m
        };

        var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/v1/jogos", content);
        var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        error.Should().NotBeNull();
        error.Message.Should().Be("Um jogo com este nome já está cadastrado no sistema.");
    }

    [Fact(DisplayName = "Deve retornar Unauthorized quando token está faltando")]
    [Trait("Api", "JogosController")]
    public async Task Quando_TokenFaltando_Cadastrar_Entao_RetornaUnauthorized()
    {
        // Arrange
        var dto = new CadastrarJogoDto
        {
            Nome = "Jogo Teste",
            Descricao = "Descrição do jogo teste",
            Preco = 99.99m
        };
        var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.PostAsync("/api/v1/jogos", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
