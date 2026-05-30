using AgroSatMonitor.API.DTOs;

namespace AgroSatMonitor.API.Interfaces
{
    public interface IVegetacaoService
    {
        Task<MonitoramentoVegetacaoResponseDto> ObterVegetacaoFazendaAsync(int fazendaId);
        Task<IEnumerable<MonitoramentoVegetacaoResponseDto>> ObterHistoricoVegetacaoAsync(int fazendaId);
    }
}
