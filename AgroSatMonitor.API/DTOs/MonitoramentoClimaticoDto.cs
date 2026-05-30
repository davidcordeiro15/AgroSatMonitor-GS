namespace AgroSatMonitor.API.DTOs
{
    public class MonitoramentoClimaticoResponseDto
    {
        public int Id { get; set; }
        public int FazendaId { get; set; }
        public string NomeFazenda { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Temperatura { get; set; }
        public double Umidade { get; set; }
        public double Precipitacao { get; set; }
        public double VelocidadeVento { get; set; }
        public DateTime DataLeitura { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
