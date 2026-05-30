using AgroSatMonitor.API.Data;
using AgroSatMonitor.API.DTOs;
using AgroSatMonitor.API.Entities;
using AgroSatMonitor.API.Exceptions;
using AgroSatMonitor.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroSatMonitor.API.Services
{
    public class CulturaAgricolaService : ICulturaAgricolaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CulturaAgricolaService> _logger;

        public CulturaAgricolaService(AppDbContext context, ILogger<CulturaAgricolaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CulturaAgricolaResponseDto>> ObterTodasAsync()
        {
            var culturas = await _context.Culturas
                .AsNoTracking()
                .Include(c => c.Fazenda)
                .OrderBy(c => c.Nome)
                .ToListAsync();

            return culturas.Select(MapearParaDto);
        }

        public async Task<IEnumerable<CulturaAgricolaResponseDto>> ObterPorFazendaAsync(int fazendaId)
        {
            if (!await _context.Fazendas.AnyAsync(f => f.Id == fazendaId))
                throw new FazendaNaoEncontradaException(fazendaId);

            var culturas = await _context.Culturas
                .AsNoTracking()
                .Include(c => c.Fazenda)
                .Where(c => c.FazendaId == fazendaId)
                .OrderBy(c => c.Nome)
                .ToListAsync();

            return culturas.Select(MapearParaDto);
        }

        public async Task<CulturaAgricolaResponseDto?> ObterPorIdAsync(int id)
        {
            var cultura = await _context.Culturas
                .AsNoTracking()
                .Include(c => c.Fazenda)
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new CulturaNaoEncontradaException(id);

            return MapearParaDto(cultura);
        }

        public async Task<CulturaAgricolaResponseDto> CriarAsync(CulturaAgricolaRequestDto dto)
        {
            if (!await _context.Fazendas.AnyAsync(f => f.Id == dto.FazendaId))
                throw new FazendaNaoEncontradaException(dto.FazendaId);

            _logger.LogInformation("Criando cultura {Nome} para fazenda ID={FazendaId}", dto.Nome, dto.FazendaId);

            var cultura = new CulturaAgricola
            {
                Nome = dto.Nome,
                Tipo = dto.Tipo,
                Safra = dto.Safra,
                FazendaId = dto.FazendaId
            };

            _context.Culturas.Add(cultura);
            await _context.SaveChangesAsync();

            await _context.Entry(cultura).Reference(c => c.Fazenda).LoadAsync();
            return MapearParaDto(cultura);
        }

        public async Task<CulturaAgricolaResponseDto?> AtualizarAsync(int id, CulturaAgricolaRequestDto dto)
        {
            var cultura = await _context.Culturas.FindAsync(id)
                ?? throw new CulturaNaoEncontradaException(id);

            if (!await _context.Fazendas.AnyAsync(f => f.Id == dto.FazendaId))
                throw new FazendaNaoEncontradaException(dto.FazendaId);

            cultura.Nome = dto.Nome;
            cultura.Tipo = dto.Tipo;
            cultura.Safra = dto.Safra;
            cultura.FazendaId = dto.FazendaId;

            await _context.SaveChangesAsync();
            await _context.Entry(cultura).Reference(c => c.Fazenda).LoadAsync();

            return MapearParaDto(cultura);
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var cultura = await _context.Culturas.FindAsync(id)
                ?? throw new CulturaNaoEncontradaException(id);

            _context.Culturas.Remove(cultura);
            await _context.SaveChangesAsync();
            return true;
        }

        private static CulturaAgricolaResponseDto MapearParaDto(CulturaAgricola cultura)
        {
            return new CulturaAgricolaResponseDto
            {
                Id = cultura.Id,
                Nome = cultura.Nome,
                Tipo = cultura.Tipo,
                Safra = cultura.Safra,
                FazendaId = cultura.FazendaId,
                NomeFazenda = cultura.Fazenda?.Nome ?? string.Empty
            };
        }
    }
}
