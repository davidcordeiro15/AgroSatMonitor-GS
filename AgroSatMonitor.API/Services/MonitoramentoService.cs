using AgroSatMonitor.API.Data;
using AgroSatMonitor.API.DTOs;
using AgroSatMonitor.API.Entities;
using AgroSatMonitor.API.Enums;
using AgroSatMonitor.API.Exceptions;
using AgroSatMonitor.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroSatMonitor.API.Services
{
    public class MonitoramentoService : IMonitoramentoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MonitoramentoService> _logger;

        public MonitoramentoService(AppDbContext context, ILogger<MonitoramentoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<AlertaAgricolaResponseDto>> GerarAlertasAsync(int fazendaId)
        {
            var fazenda = await _context.Fazendas
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == fazendaId)
                ?? throw new FazendaNaoEncontradaException(fazendaId);

            _logger.LogInformation("Gerando alertas para fazenda ID={FazendaId}", fazendaId);

            var alertasGerados = new List<AlertaAgricola>();

            // Busca último monitoramento climático
            var ultimoClima = await _context.MonitoramentosClimaticos
                .AsNoTracking()
                .Where(m => m.FazendaId == fazendaId)
                .OrderByDescending(m => m.DataLeitura)
                .FirstOrDefaultAsync();

            // Busca último monitoramento de vegetação
            var ultimaVegetacao = await _context.MonitoramentosVegetacao
                .AsNoTracking()
                .Where(m => m.FazendaId == fazendaId)
                .OrderByDescending(m => m.DataLeitura)
                .FirstOrDefaultAsync();

            if (ultimoClima != null)
            {
                alertasGerados.AddRange(VerificarAlertasClimaticos(fazendaId, ultimoClima));
            }

            if (ultimaVegetacao != null)
            {
                alertasGerados.AddRange(VerificarAlertasVegetacao(fazendaId, ultimaVegetacao));
            }

            if (!alertasGerados.Any())
            {
                // Sem dados de monitoramento → retorna alertas já armazenados
                return await ObterAlertasArmazenadosAsync(fazendaId, fazenda.Nome);
            }

            // Persiste novos alertas
            _context.Alertas.AddRange(alertasGerados);
            await _context.SaveChangesAsync();

            return alertasGerados.Select(a => MapearAlertaParaDto(a, fazenda.Nome));
        }

        public async Task<IEnumerable<HistoricoConsultaResponseDto>> ObterHistoricoConsultasAsync(int fazendaId)
        {
            if (!await _context.Fazendas.AnyAsync(f => f.Id == fazendaId))
                throw new FazendaNaoEncontradaException(fazendaId);

            var historico = await _context.HistoricosConsulta
                .AsNoTracking()
                .Where(h => h.FazendaId == fazendaId)
                .OrderByDescending(h => h.DataConsulta)
                .Take(50)
                .ToListAsync();

            return historico.Select(h => new HistoricoConsultaResponseDto
            {
                Id = h.Id,
                FazendaId = h.FazendaId,
                EndpointConsultado = h.EndpointConsultado,
                DataConsulta = h.DataConsulta,
                TempoRespostaMs = h.TempoRespostaMs,
                Sucesso = h.Sucesso
            });
        }

        private static List<AlertaAgricola> VerificarAlertasClimaticos(int fazendaId, MonitoramentoClimatico clima)
        {
            var alertas = new List<AlertaAgricola>();
            var agora = DateTime.UtcNow;

            // Alerta de temperatura extrema (> 38°C ou < 5°C)
            if (clima.Temperatura > 38)
            {
                alertas.Add(new AlertaAgricola
                {
                    FazendaId = fazendaId,
                    Tipo = TipoAlerta.TemperaturaExtrema,
                    Descricao = $"Temperatura muito alta: {clima.Temperatura:F1}°C. Risco de estresse térmico nas culturas. " +
                                "Recomenda-se irrigação emergencial e proteção das plantas.",
                    NivelRisco = clima.Temperatura > 42 ? NivelRisco.Critico : NivelRisco.Alto,
                    DataGeracao = agora
                });
            }
            else if (clima.Temperatura < 5)
            {
                alertas.Add(new AlertaAgricola
                {
                    FazendaId = fazendaId,
                    Tipo = TipoAlerta.TemperaturaExtrema,
                    Descricao = $"Temperatura muito baixa: {clima.Temperatura:F1}°C. Risco de geada. " +
                                "Recomenda-se cobertura das culturas sensíveis.",
                    NivelRisco = clima.Temperatura < 0 ? NivelRisco.Critico : NivelRisco.Alto,
                    DataGeracao = agora
                });
            }

            // Alerta de seca (precipitação zero e umidade < 30%)
            if (clima.Precipitacao == 0 && clima.Umidade < 30)
            {
                alertas.Add(new AlertaAgricola
                {
                    FazendaId = fazendaId,
                    Tipo = TipoAlerta.Seca,
                    Descricao = $"Condições de seca detectadas: precipitação {clima.Precipitacao:F1}mm, " +
                                $"umidade {clima.Umidade:F1}%. Recomenda-se acionamento do sistema de irrigação.",
                    NivelRisco = clima.Umidade < 15 ? NivelRisco.Critico : NivelRisco.Alto,
                    DataGeracao = agora
                });
            }

            // Alerta de chuva excessiva (> 50mm)
            if (clima.Precipitacao > 50)
            {
                alertas.Add(new AlertaAgricola
                {
                    FazendaId = fazendaId,
                    Tipo = TipoAlerta.ChuvaExcessiva,
                    Descricao = $"Precipitação excessiva: {clima.Precipitacao:F1}mm. Risco de encharcamento do solo e " +
                                "doenças fúngicas. Verifique o sistema de drenagem.",
                    NivelRisco = clima.Precipitacao > 100 ? NivelRisco.Critico : NivelRisco.Alto,
                    DataGeracao = agora
                });
            }

            // Alerta de vento forte (> 60 km/h)
            if (clima.VelocidadeVento > 60)
            {
                alertas.Add(new AlertaAgricola
                {
                    FazendaId = fazendaId,
                    Tipo = TipoAlerta.VentoForte,
                    Descricao = $"Velocidade do vento elevada: {clima.VelocidadeVento:F1} km/h. " +
                                "Risco de danos mecânicos às plantas. Suspenda pulverizações.",
                    NivelRisco = clima.VelocidadeVento > 80 ? NivelRisco.Critico : NivelRisco.Medio,
                    DataGeracao = agora
                });
            }

            return alertas;
        }

        private static List<AlertaAgricola> VerificarAlertasVegetacao(int fazendaId, MonitoramentoVegetacao veg)
        {
            var alertas = new List<AlertaAgricola>();

            // Alerta de baixa vegetação (NDVI < 0.25)
            if (veg.Ndvi < 0.25)
            {
                alertas.Add(new AlertaAgricola
                {
                    FazendaId = fazendaId,
                    Tipo = TipoAlerta.BaixaVegetacao,
                    Descricao = $"NDVI baixo detectado: {veg.Ndvi:F4}. Indica vegetação com baixo vigor ou " +
                                "solo exposto. Verifique disponibilidade de nutrientes e hídrica.",
                    NivelRisco = veg.Ndvi < 0.1 ? NivelRisco.Critico : NivelRisco.Alto,
                    DataGeracao = DateTime.UtcNow
                });
            }

            return alertas;
        }

        private async Task<IEnumerable<AlertaAgricolaResponseDto>> ObterAlertasArmazenadosAsync(int fazendaId, string nomeFazenda)
        {
            var alertas = await _context.Alertas
                .AsNoTracking()
                .Where(a => a.FazendaId == fazendaId)
                .OrderByDescending(a => a.DataGeracao)
                .Take(20)
                .ToListAsync();

            return alertas.Select(a => MapearAlertaParaDto(a, nomeFazenda));
        }

        private static AlertaAgricolaResponseDto MapearAlertaParaDto(AlertaAgricola alerta, string nomeFazenda)
        {
            return new AlertaAgricolaResponseDto
            {
                Id = alerta.Id,
                FazendaId = alerta.FazendaId,
                NomeFazenda = nomeFazenda,
                Tipo = alerta.Tipo.ToString(),
                Descricao = alerta.Descricao,
                NivelRisco = alerta.NivelRisco.ToString(),
                DataGeracao = alerta.DataGeracao
            };
        }
    }
}
