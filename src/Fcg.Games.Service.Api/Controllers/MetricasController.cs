using Fcg.Games.Service.Application.Dtos.Jogo;
using Fcg.Games.Service.Application.Interfaces;
using Fcg.Games.Service.Domain.Exceptions.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Fcg.Games.Service.Api.Controllers;

/// <summary>
/// Responsável pelos endpoints de gerenciamento de promoções de jogos.
/// </summary>
[ApiController]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MetricasController : MainController
{
    private readonly IMetricaAppService _service;

    public MetricasController(IMetricaAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtém uma lista dos jogos mais populares com base no número de vendas.
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna um ranking dos jogos mais vendidos. A popularidade é determinada
    /// pela contagem de transações de compra concluídas para cada jogo.
    /// </remarks>
    /// <param name="top">O número de jogos a serem retornados no ranking. O valor padrão é 5 se não for especificado.</param>
    /// <returns>Retorna 200 OK com uma lista de JogoDto ordenada pela popularidade.</returns>
    [SwaggerOperation(
        Summary = "Lista os jogos mais populares.",
        Description = "Retorna um ranking dos jogos mais vendidos na plataforma. A quantidade de jogos no ranking pode ser controlada pelo parâmetro 'top'."
    )]
    [ProducesResponseType(typeof(List<JogoDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    [Authorize]
    [HttpGet("mais-populares")]
    public async Task<IActionResult> ObterMaisPopulares([FromQuery] int top = 5)
    {
        var jogosPopulares = await _service.ObterJogosMaisPopularesAsync(top);
        return Ok(jogosPopulares);
    }
}