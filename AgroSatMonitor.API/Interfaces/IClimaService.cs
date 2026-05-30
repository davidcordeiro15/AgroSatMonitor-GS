using AgroSatMonitor.API.DTOs;

namespace AgroSatMonitor.API.Interfaces
{
    public interface IClimaService
    {
        Task<MonitoramentoClimaticoResponseDto> ObterClimaFazendaAsync(int fazendaId);
        Task<IEnumerable<MonitoramentoClimaticoResponseDto>> ObterHistoricoClimaticoAsync(int fazendaId);
    }
}
