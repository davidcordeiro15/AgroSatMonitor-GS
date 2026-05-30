namespace AgroSatMonitor.API.Exceptions
{
    public class FazendaNaoEncontradaException : Exception
    {
        public FazendaNaoEncontradaException(int id)
            : base($"Fazenda com ID {id} não foi encontrada.")
        {
        }

        public FazendaNaoEncontradaException(string message)
            : base(message)
        {
        }
    }

    public class ApiExternaException : Exception
    {
        public string NomeApi { get; }

        public ApiExternaException(string nomeApi, string message)
            : base($"Erro ao consultar a API externa '{nomeApi}': {message}")
        {
            NomeApi = nomeApi;
        }

        public ApiExternaException(string nomeApi, string message, Exception innerException)
            : base($"Erro ao consultar a API externa '{nomeApi}': {message}", innerException)
        {
            NomeApi = nomeApi;
        }
    }

    public class CulturaNaoEncontradaException : Exception
    {
        public CulturaNaoEncontradaException(int id)
            : base($"Cultura agrícola com ID {id} não foi encontrada.")
        {
        }
    }

    public class CoordenadasInvalidasException : Exception
    {
        public CoordenadasInvalidasException(double latitude, double longitude)
            : base($"Coordenadas inválidas: Latitude={latitude}, Longitude={longitude}. Verifique os valores informados.")
        {
        }
    }
}
