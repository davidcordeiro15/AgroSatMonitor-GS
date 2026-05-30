namespace AgroSatMonitor.API.Entities
{
    /// <summary>
    /// Registro de dados climáticos de uma fazenda.
    /// Herda de MonitoramentoBase (herança em POO).
    /// </summary>
    public class MonitoramentoClimatico : MonitoramentoBase
    {
        public double Temperatura { get; set; }
        public double Umidade { get; set; }
        public double Precipitacao { get; set; }
        public double VelocidadeVento { get; set; }
        public DateTime DataLeitura { get; set; }
    }
}
