namespace AgroSatMonitor.API.Entities
{
    public class HistoricoConsulta
    {
        public int Id { get; set; }
        public string EndpointConsultado { get; set; } = string.Empty;
        public DateTime DataConsulta { get; set; } = DateTime.UtcNow;
        public long TempoRespostaMs { get; set; }
        public bool Sucesso { get; set; }
        public int FazendaId { get; set; }
        public Fazenda Fazenda { get; set; } = null!;
    }
}
