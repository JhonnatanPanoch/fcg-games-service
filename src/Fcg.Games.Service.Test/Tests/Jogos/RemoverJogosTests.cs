using Fcg.Games.Service.Domain.Exceptions;
using Fcg.Games.Service.Domain.Exceptions.Responses;
using Fcg.Games.Service.Test.Configurations;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Fcg.Games.Service.Test.Tests.Jogos;

public class RemoverJogosTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RemoverJogosTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenCache.GetAdminToken(factory.Services));
    }

    [Fact(DisplayName = "Deve remover um jogo existente")]
    [Trait("Api", "JogosController")]
    public async Task Quando_IdValido_Entao_RemoveJogoERetornaNoContent()
    {
        // Arrange
        var id = "8fcc98f3-0729-4d9b-9771-0be9a6255410";

        // Act
        var response = await _client.DeleteAsync($"/api/v1/jogos/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "Deve retornar NotFound ao remover jogo inexistente")]
    [Trait("Api", "JogosController")]
    public async Task Quando_IdInexistente_Remover_Entao_RetornaNotFound()
    {
        // Arrange
        var id = "8fcc98f3-0729-4d9b-9771-0be9a6255419";

        // Act
        var response = await _client.DeleteAsync($"/api/v1/jogos/{id}");
        var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        error.Should().NotBeNull();
        error.Message.Should().Be(new NotFoundException().Message);
    }

    [Fact(DisplayName = "Deve retornar erro de validação para ID vazio ao remover")]
    [Trait("Api", "JogosController")]
    public async Task Quando_IdVazio_Remover_Entao_RetornaBadRequest()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var response = await _client.DeleteAsync($"/api/v1/jogos/{id}");
        var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Should().NotBeNull();
        error.Message.Should().Be("Erros de validação");
        error.Details?.Values.SelectMany(v => v).Should().Contain("O parâmetro informado não pode ser vazio.");
    }

    [Fact(DisplayName = "Deve retornar Unauthorized ao remover sem token")]
    [Trait("Api", "JogosController")]
    public async Task Quando_TokenFaltando_Remover_Entao_RetornaUnauthorized()
    {
        // Arrange
        var id = "8fcc98f3-0729-4d9b-9771-0be9a6255410";
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.DeleteAsync($"/api/v1/jogos/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
