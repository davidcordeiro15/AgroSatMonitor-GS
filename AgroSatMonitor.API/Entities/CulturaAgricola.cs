namespace AgroSatMonitor.API.Entities
{
    public class CulturaAgricola
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Safra { get; set; } = string.Empty;
        public int FazendaId { get; set; }
        public Fazenda Fazenda { get; set; } = null!;
    }
}
