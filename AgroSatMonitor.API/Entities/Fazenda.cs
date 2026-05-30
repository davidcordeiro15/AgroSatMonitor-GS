namespace AgroSatMonitor.API.Entities
{
    public class Fazenda
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AreaHectares { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public ICollection<CulturaAgricola> Culturas { get; set; } = new List<CulturaAgricola>();
        public ICollection<MonitoramentoClimatico> MonitoramentosClimaticos { get; set; } = new List<MonitoramentoClimatico>();
        public ICollection<MonitoramentoVegetacao> MonitoramentosVegetacao { get; set; } = new List<MonitoramentoVegetacao>();
        public ICollection<AlertaAgricola> Alertas { get; set; } = new List<AlertaAgricola>();
        public ICollection<HistoricoConsulta> HistoricosConsulta { get; set; } = new List<HistoricoConsulta>();
    }
}
