using Fcg.Games.Service.Application.Dtos.Jogo;
using Fcg.Games.Service.Test.Configurations;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Fcg.Games.Service.Test.Tests.Jogos;

public class ObterTodosTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ObterTodosTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenCache.GetUserToken(factory.Services));
    }

    [Fact(DisplayName = "Deve retornar todos os jogos")]
    [Trait("Api", "JogosController")]
    public async Task Quando_ExistemJogos_Entao_RetornaListaDeJogos()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/jogos");
        var content = JsonConvert.DeserializeObject<List<JogoDto>>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Should().HaveCountGreaterThan(0);
    }

    [Fact(DisplayName = "Deve retornar Unauthorized quando token está faltando")]
    [Trait("Api", "JogosController")]
    public async Task Quando_TokenFaltando_ObterTodos_Entao_RetornaUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/jogos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
