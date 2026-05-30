using AgroSatMonitor.API.DTOs;
using AgroSatMonitor.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgroSatMonitor.API.Controllers
{
    [ApiController]
    [Route("api/monitoramento")]
    [Produces("application/json")]
    public class MonitoramentoController : ControllerBase
    {
        private readonly IClimaService _climaService;
        private readonly IVegetacaoService _vegetacaoService;
        private readonly IMonitoramentoService _monitoramentoService;
        private readonly ILogger<MonitoramentoController> _logger;

        public MonitoramentoController(
            IClimaService climaService,
            IVegetacaoService vegetacaoService,
            IMonitoramentoService monitoramentoService,
            ILogger<MonitoramentoController> logger)
        {
            _climaService = climaService;
            _vegetacaoService = vegetacaoService;
            _monitoramentoService = monitoramentoService;
            _logger = logger;
        }

        /// <summary>
        /// Consulta o clima atual de uma fazenda via API Open-Meteo.
        /// Salva o resultado no banco e registra histórico da consulta.
        /// </summary>
        /// <param name="fazendaId">ID da fazenda.</param>
        [HttpGet("clima/{fazendaId:int}")]
        [ProducesResponseType(typeof(MonitoramentoClimaticoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> ObterClima(int fazendaId)
        {
            _logger.LogInformation("Requisição de clima para fazenda ID={FazendaId}", fazendaId);
            var dados = await _climaService.ObterClimaFazendaAsync(fazendaId);
            return Ok(dados);
        }

        /// <summary>
        /// Retorna o histórico de monitoramentos climáticos de uma fazenda (últimos 30 registros).
        /// </summary>
        /// <param name="fazendaId">ID da fazenda.</param>
        [HttpGet("clima/{fazendaId:int}/historico")]
        [ProducesResponseType(typeof(IEnumerable<MonitoramentoClimaticoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterHistoricoClima(int fazendaId)
        {
            var historico = await _climaService.ObterHistoricoClimaticoAsync(fazendaId);
            return Ok(historico);
        }

        /// <summary>
        /// Calcula o índice de vegetação NDVI de uma fazenda com base em dados climáticos.
        /// Salva o resultado no banco e registra histórico da consulta.
        /// </summary>
        /// <param name="fazendaId">ID da fazenda.</param>
        [HttpGet("vegetacao/{fazendaId:int}")]
        [ProducesResponseType(typeof(MonitoramentoVegetacaoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> ObterVegetacao(int fazendaId)
        {
            _logger.LogInformation("Requisição de vegetação para fazenda ID={FazendaId}", fazendaId);
            var dados = await _vegetacaoService.ObterVegetacaoFazendaAsync(fazendaId);
            return Ok(dados);
        }

        /// <summary>
        /// Retorna o histórico de monitoramentos de vegetação de uma fazenda (últimos 30 registros).
        /// </summary>
        /// <param name="fazendaId">ID da fazenda.</param>
        [HttpGet("vegetacao/{fazendaId:int}/historico")]
        [ProducesResponseType(typeof(IEnumerable<MonitoramentoVegetacaoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterHistoricoVegetacao(int fazendaId)
        {
            var historico = await _vegetacaoService.ObterHistoricoVegetacaoAsync(fazendaId);
            return Ok(historico);
        }

        /// <summary>
        /// Retorna o histórico de todas as consultas realizadas para uma fazenda.
        /// </summary>
        /// <param name="fazendaId">ID da fazenda.</param>
        [HttpGet("historico/{fazendaId:int}")]
        [ProducesResponseType(typeof(IEnumerable<HistoricoConsultaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterHistoricoConsultas(int fazendaId)
        {
            var historico = await _monitoramentoService.ObterHistoricoConsultasAsync(fazendaId);
            return Ok(historico);
        }
    }
}
