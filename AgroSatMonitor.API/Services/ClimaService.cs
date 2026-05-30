using AgroSatMonitor.API.Data;
using AgroSatMonitor.API.DTOs;
using AgroSatMonitor.API.Entities;
using AgroSatMonitor.API.Exceptions;
using AgroSatMonitor.API.ExternalServices;
using AgroSatMonitor.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AgroSatMonitor.API.Services
{
    public class ClimaService : IClimaService
    {
        private readonly AppDbContext _context;
        private readonly ClimaApiClient _climaApiClient;
        private readonly ILogger<ClimaService> _logger;

        public ClimaService(AppDbContext context, ClimaApiClient climaApiClient, ILogger<ClimaService> logger)
        {
            _context = context;
            _climaApiClient = climaApiClient;
            _logger = logger;
        }

        public async Task<MonitoramentoClimaticoResponseDto> ObterClimaFazendaAsync(int fazendaId)
        {
            var fazenda = await _context.Fazendas
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == fazendaId)
                ?? throw new FazendaNaoEncontradaException(fazendaId);

            _logger.LogInformation("Consultando clima para fazenda {Nome} ({Lat},{Lon})",
                fazenda.Nome, fazenda.Latitude, fazenda.Longitude);

            var sw = Stopwatch.StartNew();
            bool sucesso = false;
            DadosClimaticosBrutos? dados = null;

            try
            {
                dados = await _climaApiClient.ObterDadosClimaticoAsync(fazenda.Latitude, fazenda.Longitude);
                sucesso = true;
            }
            finally
            {
                sw.Stop();
                await SalvarHistoricoAsync(fazendaId, "GET /api/monitoramento/clima/{fazendaId}", sw.ElapsedMilliseconds, sucesso);
            }

            var monitoramento = new MonitoramentoClimatico
            {
                FazendaId = fazendaId,
                Latitude = fazenda.Latitude,
                Longitude = fazenda.Longitude,
                Temperatura = dados!.Temperatura,
                Umidade = dados.Umidade,
                Precipitacao = dados.Precipitacao,
                VelocidadeVento = dados.VelocidadeVento,
                DataLeitura = dados.DataLeitura,
                DataCriacao = DateTime.UtcNow
            };

            _context.MonitoramentosClimaticos.Add(monitoramento);
            await _context.SaveChangesAsync();

            return MapearParaDto(monitoramento, fazenda.Nome);
        }

        public async Task<IEnumerable<MonitoramentoClimaticoResponseDto>> ObterHistoricoClimaticoAsync(int fazendaId)
        {
            var fazenda = await _context.Fazendas
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == fazendaId)
                ?? throw new FazendaNaoEncontradaException(fazendaId);

            var historico = await _context.MonitoramentosClimaticos
                .AsNoTracking()
                .Where(m => m.FazendaId == fazendaId)
                .OrderByDescending(m => m.DataLeitura)
                .Take(30)
                .ToListAsync();

            return historico.Select(m => MapearParaDto(m, fazenda.Nome));
        }

        private async Task SalvarHistoricoAsync(int fazendaId, string endpoint, long tempoMs, bool sucesso)
        {
            var historico = new HistoricoConsulta
            {
                FazendaId = fazendaId,
                EndpointConsultado = endpoint,
                DataConsulta = DateTime.UtcNow,
                TempoRespostaMs = tempoMs,
                Sucesso = sucesso
            };
            _context.HistoricosConsulta.Add(historico);
            await _context.SaveChangesAsync();
        }

        private static MonitoramentoClimaticoResponseDto MapearParaDto(MonitoramentoClimatico m, string nomeFazenda)
        {
            return new MonitoramentoClimaticoResponseDto
            {
                Id = m.Id,
                FazendaId = m.FazendaId,
                NomeFazenda = nomeFazenda,
                Latitude = m.Latitude,
                Longitude = m.Longitude,
                Temperatura = m.Temperatura,
                Umidade = m.Umidade,
                Precipitacao = m.Precipitacao,
                VelocidadeVento = m.VelocidadeVento,
                DataLeitura = m.DataLeitura,
                DataCriacao = m.DataCriacao
            };
        }
    }
}
