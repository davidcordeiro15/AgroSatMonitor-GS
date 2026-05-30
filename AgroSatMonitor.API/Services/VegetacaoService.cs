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
    public class VegetacaoService : IVegetacaoService
    {
        private readonly AppDbContext _context;
        private readonly VegetacaoApiClient _vegetacaoApiClient;
        private readonly ILogger<VegetacaoService> _logger;

        public VegetacaoService(AppDbContext context, VegetacaoApiClient vegetacaoApiClient, ILogger<VegetacaoService> logger)
        {
            _context = context;
            _vegetacaoApiClient = vegetacaoApiClient;
            _logger = logger;
        }

        public async Task<MonitoramentoVegetacaoResponseDto> ObterVegetacaoFazendaAsync(int fazendaId)
        {
            var fazenda = await _context.Fazendas
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == fazendaId)
                ?? throw new FazendaNaoEncontradaException(fazendaId);

            _logger.LogInformation("Calculando NDVI para fazenda {Nome}", fazenda.Nome);

            var sw = Stopwatch.StartNew();
            bool sucesso = false;
            DadosVegetacaoBrutos? dados = null;

            try
            {
                dados = await _vegetacaoApiClient.ObterDadosVegetacaoAsync(fazenda.Latitude, fazenda.Longitude);
                sucesso = true;
            }
            finally
            {
                sw.Stop();
                await SalvarHistoricoAsync(fazendaId, "GET /api/monitoramento/vegetacao/{fazendaId}", sw.ElapsedMilliseconds, sucesso);
            }

            var monitoramento = new MonitoramentoVegetacao
            {
                FazendaId = fazendaId,
                Latitude = fazenda.Latitude,
                Longitude = fazenda.Longitude,
                Ndvi = dados!.Ndvi,
                NivelSaudeVegetacao = dados.NivelSaudeVegetacao,
                DataLeitura = dados.DataLeitura,
                DataCriacao = DateTime.UtcNow
            };

            _context.MonitoramentosVegetacao.Add(monitoramento);
            await _context.SaveChangesAsync();

            return MapearParaDto(monitoramento, fazenda.Nome);
        }

        public async Task<IEnumerable<MonitoramentoVegetacaoResponseDto>> ObterHistoricoVegetacaoAsync(int fazendaId)
        {
            var fazenda = await _context.Fazendas
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == fazendaId)
                ?? throw new FazendaNaoEncontradaException(fazendaId);

            var historico = await _context.MonitoramentosVegetacao
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

        private static MonitoramentoVegetacaoResponseDto MapearParaDto(MonitoramentoVegetacao m, string nomeFazenda)
        {
            return new MonitoramentoVegetacaoResponseDto
            {
                Id = m.Id,
                FazendaId = m.FazendaId,
                NomeFazenda = nomeFazenda,
                Latitude = m.Latitude,
                Longitude = m.Longitude,
                Ndvi = m.Ndvi,
                NivelSaudeVegetacao = m.NivelSaudeVegetacao.ToString(),
                InterpretacaoNdvi = InterpretarNdvi(m.Ndvi),
                DataLeitura = m.DataLeitura,
                DataCriacao = m.DataCriacao
            };
        }

        private static string InterpretarNdvi(double ndvi)
        {
            return ndvi switch
            {
                < 0.0 => "Água ou superfícies sem vegetação",
                < 0.1 => "Solo exposto ou vegetação muito esparsa — situação crítica",
                < 0.25 => "Vegetação com baixo vigor — intervenção necessária",
                < 0.45 => "Vegetação com vigor moderado — monitorar",
                < 0.65 => "Vegetação saudável — condições boas",
                _ => "Vegetação com alto vigor — condições excelentes"
            };
        }
    }
}
