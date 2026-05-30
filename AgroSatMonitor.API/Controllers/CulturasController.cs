using AgroSatMonitor.API.DTOs;
using AgroSatMonitor.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgroSatMonitor.API.Controllers
{
    [ApiController]
    [Route("api/culturas")]
    [Produces("application/json")]
    public class CulturasController : ControllerBase
    {
        private readonly ICulturaAgricolaService _culturaService;
        private readonly ILogger<CulturasController> _logger;

        public CulturasController(ICulturaAgricolaService culturaService, ILogger<CulturasController> logger)
        {
            _culturaService = culturaService;
            _logger = logger;
        }

        /// <summary>Lista todas as culturas agrícolas cadastradas.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CulturaAgricolaResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodas()
        {
            var culturas = await _culturaService.ObterTodasAsync();
            return Ok(culturas);
        }

        /// <summary>Lista culturas agrícolas de uma fazenda específica.</summary>
        /// <param name="fazendaId">ID da fazenda.</param>
        [HttpGet("fazenda/{fazendaId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CulturaAgricolaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorFazenda(int fazendaId)
        {
            var culturas = await _culturaService.ObterPorFazendaAsync(fazendaId);
            return Ok(culturas);
        }

        /// <summary>Retorna uma cultura agrícola pelo ID.</summary>
        /// <param name="id">ID da cultura.</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CulturaAgricolaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var cultura = await _culturaService.ObterPorIdAsync(id);
            return Ok(cultura);
        }

        /// <summary>Cadastra uma nova cultura agrícola.</summary>
        /// <param name="dto">Dados da cultura.</param>
        [HttpPost]
        [ProducesResponseType(typeof(CulturaAgricolaResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Criar([FromBody] CulturaAgricolaRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var criada = await _culturaService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
        }

        /// <summary>Atualiza os dados de uma cultura agrícola.</summary>
        /// <param name="id">ID da cultura.</param>
        /// <param name="dto">Novos dados da cultura.</param>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CulturaAgricolaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] CulturaAgricolaRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var atualizada = await _culturaService.AtualizarAsync(id, dto);
            return Ok(atualizada);
        }

        /// <summary>Remove uma cultura agrícola pelo ID.</summary>
        /// <param name="id">ID da cultura.</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(int id)
        {
            await _culturaService.ExcluirAsync(id);
            return NoContent();
        }
    }
}
