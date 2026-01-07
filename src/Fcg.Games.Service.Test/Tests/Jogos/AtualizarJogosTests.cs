using Fcg.Games.Service.Application.Dtos.Jogo;
using Fcg.Games.Service.Domain.Exceptions;
using Fcg.Games.Service.Domain.Exceptions.Responses;
using Fcg.Games.Service.Test.Configurations;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Fcg.Games.Service.Test.Tests.Jogos;

public class AtualizarJogosTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AtualizarJogosTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenCache.GetAdminToken(factory.Services));
    }

    [Fact(DisplayName = "Deve atualizar um jogo existente")]
    [Trait("Api", "JogosController")]
    public async Task Quando_DadosValidos_Entao_AtualizaJogoERetornaNoContent()
    {
        // Arrange
        var id = "32e60c8c-d5bb-4378-afdd-28c07cc71b91";
        var dto = new AtualizarJogoDto
        {
            Nome = "Jogo Atualizado",
            Descricao = "Descrição atualizada",
            Preco = 199.99m,
        };
        var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/jogos/{id}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "Deve retornar Conflict ao atualizar jogo com nome já existente")]
    [Trait("Api", "JogosController")]
    public async Task Quando_NomeJaExiste_DeveRetornarConflict()
    {
        // Arrange
        var id = "32e60c8c-d5bb-4378-afdd-28c07cc71b91";
        var dto = new AtualizarJogoDto
        {
            Nome = $"Jogo 2",
            Descricao = "Descrição do jogo duplicado",
            Preco = 59.99m
        };
        var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/jogos/{id}", content);
        var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        error.Should().NotBeNull();
        error.Message.Should().Be("Um jogo com este nome já está cadastrado no sistema.");
    }

    [Fact(DisplayName = "Deve retornar NotFound para jogo inexistente")]
    [Trait("Api", "JogosController")]
    public async Task Quando_IdInexistente_Atualizar_Entao_RetornaNotFound()
    {
        // Arrange
        var id = "32e60c8c-d5bb-4378-afdd-28c07cc71b92";
        var dto = new AtualizarJogoDto
        {
            Nome = "Jogo",
            Descricao = "Descrição do jogo",
            Preco = 10m
        };
        var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/jogos/{id}", content);
        var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        error.Should().NotBeNull();
        error.Message.Should().Be(new NotFoundException().Message);
    }

    [Fact(DisplayName = "Deve retornar erro de validação para ID vazio")]
    [Trait("Api", "JogosController")]
    public async Task Quando_IdVazio_Atualizar_Entao_RetornaBadRequest()
    {
        // Arrange
        var id = Guid.Empty;
        var dto = new AtualizarJogoDto
        {
            Nome = "Jogo",
            Descricao = "Descrição do jogo",
            Preco = 10m
        };
        var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/jogos/{id}", content);
        var error = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Should().NotBeNull();
        error.Message.Should().Be("Erros de validação");
        error.Details?.Values.SelectMany(v => v).Should().Contain("O parâmetro informado não pode ser vazio.");
    }

    [Fact(DisplayName = "Deve retornar Unauthorized quando token está faltando")]
    [Trait("Api", "JogosController")]
    public async Task Quando_TokenFaltando_Atualizar_Entao_RetornaUnauthorized()
    {
        // Arrange
        var id = "32e60c8c-d5bb-4378-afdd-28c07cc71b91";
        var dto = new AtualizarJogoDto
        {
            Nome = "Jogo",
            Descricao = "Descrição",
            Preco = 10m
        };
        var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.PutAsync($"/api/v1/jogos/{id}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
