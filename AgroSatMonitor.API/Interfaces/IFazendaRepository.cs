using AgroSatMonitor.API.Entities;

namespace AgroSatMonitor.API.Interfaces
{
    public interface IFazendaRepository
    {
        Task<IEnumerable<Fazenda>> ObterTodosAsync();
        Task<Fazenda?> ObterPorIdAsync(int id);
        Task<Fazenda> CriarAsync(Fazenda fazenda);
        Task<Fazenda?> AtualizarAsync(Fazenda fazenda);
        Task<bool> ExcluirAsync(int id);
        Task<bool> ExisteAsync(int id);
    }
}
