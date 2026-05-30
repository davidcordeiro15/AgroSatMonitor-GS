using AgroSatMonitor.API.DTOs;

namespace AgroSatMonitor.API.Interfaces
{
    public interface ICulturaAgricolaService
    {
        Task<IEnumerable<CulturaAgricolaResponseDto>> ObterTodasAsync();
        Task<IEnumerable<CulturaAgricolaResponseDto>> ObterPorFazendaAsync(int fazendaId);
        Task<CulturaAgricolaResponseDto?> ObterPorIdAsync(int id);
        Task<CulturaAgricolaResponseDto> CriarAsync(CulturaAgricolaRequestDto dto);
        Task<CulturaAgricolaResponseDto?> AtualizarAsync(int id, CulturaAgricolaRequestDto dto);
        Task<bool> ExcluirAsync(int id);
    }
}
