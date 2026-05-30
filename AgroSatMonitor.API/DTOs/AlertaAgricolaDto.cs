using AgroSatMonitor.API.Enums;

namespace AgroSatMonitor.API.DTOs
{
    public class AlertaAgricolaResponseDto
    {
        public int Id { get; set; }
        public int FazendaId { get; set; }
        public string NomeFazenda { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string NivelRisco { get; set; } = string.Empty;
        public DateTime DataGeracao { get; set; }
    }
}
