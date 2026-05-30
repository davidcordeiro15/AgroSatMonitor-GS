namespace AgroSatMonitor.API.DTOs
{
    public class HistoricoConsultaResponseDto
    {
        public int Id { get; set; }
        public int FazendaId { get; set; }
        public string EndpointConsultado { get; set; } = string.Empty;
        public DateTime DataConsulta { get; set; }
        public long TempoRespostaMs { get; set; }
        public bool Sucesso { get; set; }
    }
}
