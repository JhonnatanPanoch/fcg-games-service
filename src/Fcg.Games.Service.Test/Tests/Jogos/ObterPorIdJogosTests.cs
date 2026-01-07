using Fcg.Games.Service.Domain.Exceptions;
using Fcg.Games.Service.Application.Dtos.Jogo;
using Fcg.Games.Service.Domain.Exceptions.Responses;
using Fcg.Games.Service.Test.Configurations;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Fcg.Games.Service.Test.Tests.Jogos;

public class ObterPorIdJogosTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ObterPorIdJogosTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenCache.GetUserToken(factory.Services));
    }

    [Fact(DisplayName = "Deve retornar um jogo pelo ID")]
    [Trait("Api", "JogosController")]
    public async Task Quando_IdValido_Entao_RetornaJogo()
    {
        // Arrange
        var id = "32e60c8c-d5bb-4378-afdd-28c07cc71b91";

        // Act
        var response = await _client.GetAsync($"/api/v1/jogos/{id}");
        var content = JsonConvert.DeserializeObject<JogoDto>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Id.Should().Be(Guid.Parse(id));
        content.Nome.Should().Be("Jogo 1");
        content.Descricao.Should().Be("Descrição do Jogo 1");
        content.Preco.Should().Be(15M);
        content.Ativo.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve retornar NotFound para jogo inexistente")]
    [Trait("Api", "JogosController")]
    public async Task Quando_IdInexistente_Entao_RetornaNotFound()
    {
        // Arrange
        var id = "32e60c8c-d5bb-4378-afdd-28c07cc71b92";

        // Act
        var response = await _client.GetAsync($"/api/v1/jogos/{id}");
        var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        error.Should().NotBeNull();
        error.Message.Should().Be(new NotFoundException().Message);
    }

    [Fact(DisplayName = "Deve retornar erro de validação para ID vazio")]
    [Trait("Api", "JogosController")]
    public async Task Quando_IdVazio_Entao_RetornaBadRequest()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var response = await _client.GetAsync($"/api/v1/jogos/{id}");
        var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Should().NotBeNull();
        error.Message.Should().Be("Erros de validação");
        error.Details?.Values.SelectMany(v => v).Should().Contain("O parâmetro informado não pode ser vazio.");
    }

    [Fact(DisplayName = "Deve retornar Unauthorized quando token está faltando")]
    [Trait("Api", "JogosController")]
    public async Task Quando_TokenFaltando_ObterPorId_Entao_RetornaUnauthorized()
    {
        // Arrange
        var id = "32e60c8c-d5bb-4378-afdd-28c07cc71b91";
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync($"/api/v1/jogos/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
