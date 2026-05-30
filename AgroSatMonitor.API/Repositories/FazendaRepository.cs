using AgroSatMonitor.API.Data;
using AgroSatMonitor.API.Entities;
using AgroSatMonitor.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroSatMonitor.API.Repositories
{
    public class FazendaRepository : IFazendaRepository
    {
        private readonly AppDbContext _context;

        public FazendaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Fazenda>> ObterTodosAsync()
        {
            return await _context.Fazendas
                .AsNoTracking()
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }

        public async Task<Fazenda?> ObterPorIdAsync(int id)
        {
            return await _context.Fazendas
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Fazenda> CriarAsync(Fazenda fazenda)
        {
            fazenda.DataCadastro = DateTime.UtcNow;
            _context.Fazendas.Add(fazenda);
            await _context.SaveChangesAsync();
            return fazenda;
        }

        public async Task<Fazenda?> AtualizarAsync(Fazenda fazenda)
        {
            var existente = await _context.Fazendas.FindAsync(fazenda.Id);
            if (existente == null) return null;

            existente.Nome = fazenda.Nome;
            existente.Latitude = fazenda.Latitude;
            existente.Longitude = fazenda.Longitude;
            existente.AreaHectares = fazenda.AreaHectares;
            existente.Cidade = fazenda.Cidade;
            existente.Estado = fazenda.Estado;

            await _context.SaveChangesAsync();
            return existente;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var fazenda = await _context.Fazendas.FindAsync(id);
            if (fazenda == null) return false;

            _context.Fazendas.Remove(fazenda);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteAsync(int id)
        {
            
            return await _context.Fazendas.CountAsync(f => f.Id == id) > 0;
        }
    }
}