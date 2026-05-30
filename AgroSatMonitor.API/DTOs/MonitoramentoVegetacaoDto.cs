using AgroSatMonitor.API.Enums;

namespace AgroSatMonitor.API.DTOs
{
    public class MonitoramentoVegetacaoResponseDto
    {
        public int Id { get; set; }
        public int FazendaId { get; set; }
        public string NomeFazenda { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Ndvi { get; set; }
        public string NivelSaudeVegetacao { get; set; } = string.Empty;
        public string InterpretacaoNdvi { get; set; } = string.Empty;
        public DateTime DataLeitura { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
