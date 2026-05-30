namespace AgroSatMonitor.API.Entities
{
    /// <summary>
    /// Classe abstrata base para todos os registros de monitoramento.
    /// Demonstra o conceito de abstração e herança em POO.
    /// </summary>
    public abstract class MonitoramentoBase
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int FazendaId { get; set; }
        public Fazenda Fazenda { get; set; } = null!;
    }
}
