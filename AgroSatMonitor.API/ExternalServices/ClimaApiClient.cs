using AgroSatMonitor.API.Exceptions;
using System.Text.Json;

namespace AgroSatMonitor.API.ExternalServices
{
    public class ClimaApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClimaApiClient> _logger;
        private const string BaseUrl = "https://api.open-meteo.com/v1/forecast";

        public ClimaApiClient(HttpClient httpClient, ILogger<ClimaApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<DadosClimaticosBrutos> ObterDadosClimaticoAsync(double latitude, double longitude)
        {
            var url = $"{BaseUrl}?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                      $"&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                      $"&current=temperature_2m,relative_humidity_2m,wind_speed_10m,precipitation" +
                      $"&timezone=America%2FSao_Paulo";

            _logger.LogInformation("Consultando Open-Meteo API: {Url}", url);

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _httpClient.GetAsync(url, cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiExternaException("Open-Meteo",
                        $"Código HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var dados = JsonSerializer.Deserialize<OpenMeteoResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (dados?.Current == null)
                {
                    throw new ApiExternaException("Open-Meteo", "Resposta sem dados climáticos.");
                }

                return new DadosClimaticosBrutos
                {
                    Temperatura = dados.Current.Temperature_2m,
                    Umidade = dados.Current.Relative_Humidity_2m,
                    VelocidadeVento = dados.Current.Wind_Speed_10m,
                    Precipitacao = dados.Current.Precipitation,
                    DataLeitura = DateTime.UtcNow
                };
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException("A consulta à API de clima excedeu o tempo limite de 10 segundos.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiExternaException("Open-Meteo", $"Falha de rede: {ex.Message}", ex);
            }
        }
    }

    // Modelos para deserializar a resposta da Open-Meteo
    internal class OpenMeteoResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public OpenMeteoCurrent? Current { get; set; }
    }

    internal class OpenMeteoCurrent
    {
        public double Temperature_2m { get; set; }
        public double Relative_Humidity_2m { get; set; }
        public double Wind_Speed_10m { get; set; }
        public double Precipitation { get; set; }
    }

    public class DadosClimaticosBrutos
    {
        public double Temperatura { get; set; }
        public double Umidade { get; set; }
        public double VelocidadeVento { get; set; }
        public double Precipitacao { get; set; }
        public DateTime DataLeitura { get; set; }
    }
}
