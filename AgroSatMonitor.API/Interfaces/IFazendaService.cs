using AgroSatMonitor.API.DTOs;

namespace AgroSatMonitor.API.Interfaces
{
    public interface IFazendaService
    {
        Task<IEnumerable<FazendaResponseDto>> ObterTodasAsync();
        Task<FazendaResponseDto?> ObterPorIdAsync(int id);
        Task<FazendaResponseDto> CriarAsync(FazendaRequestDto dto);
        Task<FazendaResponseDto?> AtualizarAsync(int id, FazendaRequestDto dto);
        Task<bool> ExcluirAsync(int id);
    }
}
