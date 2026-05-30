using AgroSatMonitor.API.DTOs;
using AgroSatMonitor.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgroSatMonitor.API.Controllers
{
    [ApiController]
    [Route("api/fazendas")]
    [Produces("application/json")]
    public class FazendasController : ControllerBase
    {
        private readonly IFazendaService _fazendaService;
        private readonly ILogger<FazendasController> _logger;

        public FazendasController(IFazendaService fazendaService, ILogger<FazendasController> logger)
        {
            _fazendaService = fazendaService;
            _logger = logger;
        }

        /// <summary>Lista todas as fazendas cadastradas.</summary>
        /// <returns>Lista de fazendas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FazendaResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodas()
        {
            var fazendas = await _fazendaService.ObterTodasAsync();
            return Ok(fazendas);
        }

        /// <summary>Retorna uma fazenda específica pelo ID.</summary>
        /// <param name="id">ID da fazenda.</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(FazendaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var fazenda = await _fazendaService.ObterPorIdAsync(id);
            return Ok(fazenda);
        }

        /// <summary>Cadastra uma nova fazenda.</summary>
        /// <param name="dto">Dados da fazenda.</param>
        [HttpPost]
        [ProducesResponseType(typeof(FazendaResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Criar([FromBody] FazendaRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var criada = await _fazendaService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
        }

        /// <summary>Atualiza os dados de uma fazenda existente.</summary>
        /// <param name="id">ID da fazenda.</param>
        /// <param name="dto">Novos dados da fazenda.</param>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(FazendaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] FazendaRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var atualizada = await _fazendaService.AtualizarAsync(id, dto);
            return Ok(atualizada);
        }

        /// <summary>Remove uma fazenda pelo ID.</summary>
        /// <param name="id">ID da fazenda.</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(int id)
        {
            await _fazendaService.ExcluirAsync(id);
            return NoContent();
        }
    }
}
