using AgroSatMonitor.API.Enums;

namespace AgroSatMonitor.API.Entities
{
    /// <summary>
    /// Registro de índice de vegetação (NDVI) de uma fazenda.
    /// Herda de MonitoramentoBase (herança em POO).
    /// </summary>
    public class MonitoramentoVegetacao : MonitoramentoBase
    {
        public double Ndvi { get; set; }
        public NivelSaudeVegetacao NivelSaudeVegetacao { get; set; }
        public DateTime DataLeitura { get; set; }
    }
}
