using Fcg.Games.Service.Application.Dtos.Jogo;
using Fcg.Games.Service.Application.Interfaces;
using Fcg.Games.Service.Domain.Exceptions.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Fcg.Games.Service.Api.Controllers
{
    /// <summary>
    /// Responsável pelos endpoints de gerenciamento de jogos.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class JogosController : MainController
    {
        private readonly IJogoAppService _service;

        public JogosController(IJogoAppService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtém a lista de todos os jogos cadastrados.
        /// </summary>
        [SwaggerOperation(
            Summary = "Lista todos os jogos.",
            Description = "Retorna uma lista com todos os jogos cadastrados no sistema."
        )]
        [ProducesResponseType(typeof(List<JogoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObterTodos()
        {
            var jogos = await _service.ObterTodosAsync();
            return Ok(jogos);
        }

        /// <summary>
        /// Obtém os detalhes de um jogo específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do jogo.</param>
        [SwaggerOperation(
            Summary = "Obtém um jogo por ID.",
            Description = "Retorna os detalhes de um jogo específico a partir do seu identificador."
        )]
        [ProducesResponseType(typeof(JogoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
        {
            var jogo = await _service.ObterPorIdAsync(id);
            return Ok(jogo);
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> ObterPorTermo([FromQuery] string query)
        {
            var jogos = await _service.BuscarJogosPorTermoAsync(query);
            return Ok(jogos);
        }

        [HttpGet("consultar-precos")]
        [Authorize]
        public async Task<IActionResult> ConsultarPrecos([FromQuery] List<Guid> ids)
        {
            var jogos = await _service.ConsultarPrecosAsync(ids);
            return Ok(new { jogos });
        }

        /// <summary>
        /// Cadastra um novo jogo no sistema.
        /// </summary>
        /// <param name="dto">Dados do novo jogo.</param>
        [ProducesResponseType(typeof((string, JogoDto)), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(
            Summary = "Cadastra um novo jogo.",
            Description = "Adiciona um novo jogo ao sistema. Requer permissão de administrador."
        )]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarJogoDto dto)
        {
            var registro = await _service.CadastrarAsync(dto);
            return Created($"/jogos/{registro.Id}", registro);
        }

        /// <summary>
        /// Atualiza os dados de um jogo existente.
        /// </summary>
        /// <param name="id">ID do jogo.</param>
        /// <param name="dto">Novos dados do jogo.</param>
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(
            Summary = "Atualiza um jogo.",
            Description = "Atualiza as informações de um jogo existente. Requer permissão de administrador."
        )]
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Atualizar(
            [FromRoute] Guid id,
            [FromBody] AtualizarJogoDto dto)
        {
            await _service.AtualizarAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// Remove um jogo do sistema.
        /// </summary>
        /// <param name="id">ID do jogo.</param>
        [SwaggerOperation(
            Summary = "Remove um jogo.",
            Description = "Remove um jogo do sistema pelo seu identificador. Requer permissão de administrador."
        )]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remover([FromRoute] Guid id)
        {
            await _service.RemoverAsync(id);
            return NoContent();
        }
    }
}
