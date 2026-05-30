using AgroSatMonitor.API.DTOs;
using AgroSatMonitor.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgroSatMonitor.API.Controllers
{
    [ApiController]
    [Route("api/alertas")]
    [Produces("application/json")]
    public class AlertasController : ControllerBase
    {
        private readonly IMonitoramentoService _monitoramentoService;
        private readonly ILogger<AlertasController> _logger;

        public AlertasController(IMonitoramentoService monitoramentoService, ILogger<AlertasController> logger)
        {
            _monitoramentoService = monitoramentoService;
            _logger = logger;
        }

        /// <summary>
        /// Gera e retorna alertas agrícolas para uma fazenda.
        /// Analisa os últimos dados climáticos e de vegetação cadastrados,
        /// gerando alertas automáticos de seca, temperatura extrema, baixa vegetação,
        /// chuva excessiva e vento forte.
        /// </summary>
        /// <param name="fazendaId">ID da fazenda.</param>
        [HttpGet("{fazendaId:int}")]
        [ProducesResponseType(typeof(IEnumerable<AlertaAgricolaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GerarAlertas(int fazendaId)
        {
            _logger.LogInformation("Gerando alertas para fazenda ID={FazendaId}", fazendaId);
            var alertas = await _monitoramentoService.GerarAlertasAsync(fazendaId);
            return Ok(alertas);
        }
    }
}
