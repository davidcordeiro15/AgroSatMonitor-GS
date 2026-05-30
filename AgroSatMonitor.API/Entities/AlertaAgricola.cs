using AgroSatMonitor.API.Enums;

namespace AgroSatMonitor.API.Entities
{
    public class AlertaAgricola
    {
        public int Id { get; set; }
        public TipoAlerta Tipo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public NivelRisco NivelRisco { get; set; }
        public DateTime DataGeracao { get; set; } = DateTime.UtcNow;
        public int FazendaId { get; set; }
        public Fazenda Fazenda { get; set; } = null!;
    }
}
