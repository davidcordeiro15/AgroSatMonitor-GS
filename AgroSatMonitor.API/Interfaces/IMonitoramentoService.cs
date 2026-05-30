using AgroSatMonitor.API.DTOs;

namespace AgroSatMonitor.API.Interfaces
{
    public interface IMonitoramentoService
    {
        Task<IEnumerable<AlertaAgricolaResponseDto>> GerarAlertasAsync(int fazendaId);
        Task<IEnumerable<HistoricoConsultaResponseDto>> ObterHistoricoConsultasAsync(int fazendaId);
    }
}
