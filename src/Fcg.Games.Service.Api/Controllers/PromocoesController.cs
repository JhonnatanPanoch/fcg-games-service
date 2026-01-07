using Fcg.Games.Service.Application.Dtos.Promocao;
using Fcg.Games.Service.Application.Interfaces;
using Fcg.Games.Service.Domain.Exceptions.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Fcg.Games.Service.Api.Controllers
{
    /// <summary>
    /// Responsável pelos endpoints de gerenciamento de promoções de jogos.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PromocoesController : MainController
    {
        private readonly IPromocaoAppService _service;

        public PromocoesController(IPromocaoAppService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtém os detalhes de uma promoção específica pelo seu ID.
        /// </summary>
        /// <param name="id">ID da promoção.</param>
        [SwaggerOperation(
            Summary = "Obtém uma promoção por ID.",
            Description = "Retorna os detalhes de uma promoção específica a partir do seu identificador."
        )]
        [ProducesResponseType(typeof(PromocaoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var promocao = await _service.ObterPorIdAsync(id);
            return Ok(promocao);
        }

        /// <summary>
        /// Lista todas as promoções cadastradas.
        /// </summary>
        [SwaggerOperation(
            Summary = "Lista todas as promoções.",
            Description = "Retorna uma lista com todas as promoções cadastradas no sistema."
        )]
        [ProducesResponseType(typeof(List<PromocaoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObterTodos()
        {
            var promocoes = await _service.ObterTodosAsync();
            return Ok(promocoes);
        }

        /// <summary>
        /// Cadastra uma nova promoção para um jogo.
        /// </summary>
        /// <param name="role">Dados da nova promoção.</param>
        [SwaggerOperation(
            Summary = "Cadastra uma nova promoção.",
            Description = "Adiciona uma nova promoção para um jogo no sistema."
        )]
        [ProducesResponseType(typeof((string, PromocaoDto)), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarPromocaoDto role)
        {
            var promo = await _service.CadastrarAsync(role);
            return Created($"api/v1/promocoes/{promo.Id}", promo);
        }

        /// <summary>
        /// Remove uma promoção do sistema.
        /// </summary>
        /// <param name="id">ID da promoção.</param>
        [SwaggerOperation(
            Summary = "Remove uma promoção.",
            Description = "Remove uma promoção do sistema pelo seu identificador."
        )]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remover(Guid id)
        {
            await _service.RemoverAsync(id);
            return NoContent();
        }
    }
}