using AgroSatMonitor.API.DTOs;
using AgroSatMonitor.API.Entities;
using AgroSatMonitor.API.Exceptions;
using AgroSatMonitor.API.Interfaces;
using AgroSatMonitor.API.Utils;

namespace AgroSatMonitor.API.Services
{
    public class FazendaService : IFazendaService
    {
        private readonly IFazendaRepository _repository;
        private readonly ILogger<FazendaService> _logger;

        public FazendaService(IFazendaRepository repository, ILogger<FazendaService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<FazendaResponseDto>> ObterTodasAsync()
        {
            _logger.LogInformation("Buscando todas as fazendas.");
            var fazendas = await _repository.ObterTodosAsync();
            return fazendas.Select(MapearParaDto);
        }

        public async Task<FazendaResponseDto?> ObterPorIdAsync(int id)
        {
            _logger.LogInformation("Buscando fazenda ID={Id}", id);
            var fazenda = await _repository.ObterPorIdAsync(id);
            if (fazenda == null)
                throw new FazendaNaoEncontradaException(id);

            return MapearParaDto(fazenda);
        }

        public async Task<FazendaResponseDto> CriarAsync(FazendaRequestDto dto)
        {
            CoordenadasValidator.Validar(dto.Latitude, dto.Longitude);
            _logger.LogInformation("Criando fazenda: {Nome}", dto.Nome);

            var fazenda = new Fazenda
            {
                Nome = dto.Nome,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                AreaHectares = dto.AreaHectares,
                Cidade = dto.Cidade,
                Estado = dto.Estado.ToUpper()
            };

            var criada = await _repository.CriarAsync(fazenda);
            return MapearParaDto(criada);
        }

        public async Task<FazendaResponseDto?> AtualizarAsync(int id, FazendaRequestDto dto)
        {
            CoordenadasValidator.Validar(dto.Latitude, dto.Longitude);
            _logger.LogInformation("Atualizando fazenda ID={Id}", id);

            if (!await _repository.ExisteAsync(id))
                throw new FazendaNaoEncontradaException(id);

            var fazenda = new Fazenda
            {
                Id = id,
                Nome = dto.Nome,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                AreaHectares = dto.AreaHectares,
                Cidade = dto.Cidade,
                Estado = dto.Estado.ToUpper()
            };

            var atualizada = await _repository.AtualizarAsync(fazenda);
            return atualizada != null ? MapearParaDto(atualizada) : null;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            _logger.LogInformation("Excluindo fazenda ID={Id}", id);
            if (!await _repository.ExisteAsync(id))
                throw new FazendaNaoEncontradaException(id);

            return await _repository.ExcluirAsync(id);
        }

        private static FazendaResponseDto MapearParaDto(Fazenda fazenda)
        {
            return new FazendaResponseDto
            {
                Id = fazenda.Id,
                Nome = fazenda.Nome,
                Latitude = fazenda.Latitude,
                Longitude = fazenda.Longitude,
                AreaHectares = fazenda.AreaHectares,
                Cidade = fazenda.Cidade,
                Estado = fazenda.Estado,
                DataCadastro = fazenda.DataCadastro
            };
        }
    }
}
