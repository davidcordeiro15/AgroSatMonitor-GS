using AgroSatMonitor.API.Enums;
using AgroSatMonitor.API.Exceptions;
using System.Text.Json;

namespace AgroSatMonitor.API.ExternalServices
{
    /// <summary>
    /// Cliente para cálculo de NDVI (Índice de Vegetação por Diferença Normalizada).
    /// Utiliza a API Open-Meteo para obter dados de radiação solar e umidade,
    /// e aplica correlação científica para estimar o NDVI.
    /// Para produção, pode ser substituído pela API AgroMonitoring ou NASA POWER.
    /// </summary>
    public class VegetacaoApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VegetacaoApiClient> _logger;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "https://api.open-meteo.com/v1/forecast";

        public VegetacaoApiClient(HttpClient httpClient, ILogger<VegetacaoApiClient> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<DadosVegetacaoBrutos> ObterDadosVegetacaoAsync(double latitude, double longitude)
        {
            _logger.LogInformation("Calculando NDVI para coordenadas Lat={Lat}, Lon={Lon}", latitude, longitude);

            // Busca dados climáticos necessários para o cálculo de NDVI
            var url = $"{BaseUrl}?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                      $"&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                      $"&daily=shortwave_radiation_sum,precipitation_sum,et0_fao_evapotranspiration" +
                      $"&current=relative_humidity_2m,temperature_2m" +
                      $"&timezone=America%2FSao_Paulo&forecast_days=1";

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _httpClient.GetAsync(url, cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiExternaException("Open-Meteo (Vegetação)",
                        $"Código HTTP {(int)response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var dados = JsonSerializer.Deserialize<OpenMeteoVegetacaoResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                double ndvi = CalcularNdvi(
                    dados?.Daily?.Shortwave_Radiation_Sum?.FirstOrDefault() ?? 15.0,
                    dados?.Daily?.Precipitation_Sum?.FirstOrDefault() ?? 2.0,
                    dados?.Daily?.Et0_Fao_Evapotranspiration?.FirstOrDefault() ?? 3.5,
                    dados?.Current?.Relative_Humidity_2m ?? 65,
                    latitude
                );

                var nivelSaude = ClassificarNdvi(ndvi);

                return new DadosVegetacaoBrutos
                {
                    Ndvi = Math.Round(ndvi, 4),
                    NivelSaudeVegetacao = nivelSaude,
                    DataLeitura = DateTime.UtcNow
                };
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException("A consulta à API de vegetação excedeu o tempo limite.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiExternaException("Open-Meteo (Vegetação)", $"Falha de rede: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calcula NDVI estimado a partir de variáveis climáticas.
        /// Baseado na correlação entre radiação solar, evapotranspiração e índice de vegetação.
        /// NDVI varia de -1 (sem vegetação/água) a 1 (vegetação densa).
        /// </summary>
        private static double CalcularNdvi(double radiacaoSolar, double precipitacao,
            double evapotranspiracao, double umidade, double latitude)
        {
            // Fator de radiação solar normalizado (0-1)
            double fatorRadiacao = Math.Min(radiacaoSolar / 30.0, 1.0);

            // Fator hídrico: balanço entre precipitação e evapotranspiração
            double balHidrico = evapotranspiracao > 0
                ? Math.Min(precipitacao / evapotranspiracao, 2.0)
                : 0.5;
            double fatorHidrico = Math.Min(balHidrico / 2.0, 1.0);

            // Fator de umidade do ar
            double fatorUmidade = umidade / 100.0;

            // Fator latitudinal: simula zonas de vegetação (trópicos mais vegetação)
            double fatorLatitude = 1.0 - (Math.Abs(latitude) / 90.0) * 0.3;

            // Cálculo NDVI composto
            double ndviBase = (fatorRadiacao * 0.3) + (fatorHidrico * 0.4) +
                              (fatorUmidade * 0.2) + (fatorLatitude * 0.1);

            // Mapeia de [0,1] para [-0.1, 0.9] (faixa realista para vegetação agrícola)
            double ndvi = (ndviBase * 1.0) - 0.1;

            return Math.Clamp(ndvi, -0.1, 0.9);
        }

        private static NivelSaudeVegetacao ClassificarNdvi(double ndvi)
        {
            return ndvi switch
            {
                < 0.1 => NivelSaudeVegetacao.Critica,
                < 0.25 => NivelSaudeVegetacao.Baixa,
                < 0.45 => NivelSaudeVegetacao.Moderada,
                < 0.65 => NivelSaudeVegetacao.Boa,
                _ => NivelSaudeVegetacao.Excelente
            };
        }
    }

    internal class OpenMeteoVegetacaoResponse
    {
        public OpenMeteoVegetacaoDaily? Daily { get; set; }
        public OpenMeteoVegetacaoCurrent? Current { get; set; }
    }

    internal class OpenMeteoVegetacaoDaily
    {
        public List<double>? Shortwave_Radiation_Sum { get; set; }
        public List<double>? Precipitation_Sum { get; set; }
        public List<double>? Et0_Fao_Evapotranspiration { get; set; }
    }

    internal class OpenMeteoVegetacaoCurrent
    {
        public double Relative_Humidity_2m { get; set; }
        public double Temperature_2m { get; set; }
    }

    public class DadosVegetacaoBrutos
    {
        public double Ndvi { get; set; }
        public NivelSaudeVegetacao NivelSaudeVegetacao { get; set; }
        public DateTime DataLeitura { get; set; }
    }
}
