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
public class SugestoesController : MainController
{
    private readonly ISugestaoAppService _service;
    private readonly IUsuarioAutenticadoAppService _userAppService;

    public SugestoesController(
        ISugestaoAppService service,
        IUsuarioAutenticadoAppService userAppService)
    {
        _service = service;
        _userAppService = userAppService;
    }

    /// <summary>
    /// Obtém uma lista de sugestões de jogos personalizadas para o usuário autenticado.
    /// </summary>
    /// <remarks>
    /// A lógica de sugestão é baseada no histórico de compras do usuário. Para novos usuários sem histórico,
    /// o endpoint pode retornar uma lista dos jogos mais populares como sugestão padrão.
    /// </remarks>
    /// <returns>Retorna 200 OK com uma lista de JogoDto contendo as sugestões.</returns>
    [SwaggerOperation(
        Summary = "Obtém sugestões de jogos para o usuário logado.",
        Description = "Retorna uma lista de até 5 jogos sugeridos para o usuário autenticado, com base em seu histórico de compras e similaridade com outros jogos."
    )]
    [ProducesResponseType(typeof(List<JogoDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ObterSugestoes()
    {
        var sugestoes = await _service.ObterSugestoesParaUsuarioAsync(_userAppService.ObterIdUsuario());
        return Ok(sugestoes);
    }
}