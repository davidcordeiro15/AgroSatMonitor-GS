using AgroSatMonitor.API.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace AgroSatMonitor.API.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
                await TratarExcecaoAsync(context, ex);
            }
        }

        private static async Task TratarExcecaoAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var resposta = new ErroResposta();

            switch (exception)
            {
                case FazendaNaoEncontradaException:
                case CulturaNaoEncontradaException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    resposta.StatusCode = context.Response.StatusCode;
                    resposta.Mensagem = exception.Message;
                    break;

                case CoordenadasInvalidasException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    resposta.StatusCode = context.Response.StatusCode;
                    resposta.Mensagem = exception.Message;
                    break;

                case ApiExternaException apiEx:
                    context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    resposta.StatusCode = context.Response.StatusCode;
                    resposta.Mensagem = apiEx.Message;
                    break;

                case HttpRequestException httpEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                    resposta.StatusCode = context.Response.StatusCode;
                    resposta.Mensagem = $"Falha na comunicação com serviço externo: {httpEx.Message}";
                    break;

                case TimeoutException:
                    context.Response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
                    resposta.StatusCode = context.Response.StatusCode;
                    resposta.Mensagem = "A requisição ao serviço externo excedeu o tempo limite.";
                    break;

                case DbUpdateException dbEx:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    resposta.StatusCode = context.Response.StatusCode;
                    resposta.Mensagem = $"Erro ao persistir dados no banco de dados: {dbEx.InnerException?.Message ?? dbEx.Message}";
                    break;

                case FormatException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    resposta.StatusCode = context.Response.StatusCode;
                    resposta.Mensagem = $"Formato de dado inválido: {exception.Message}";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    resposta.StatusCode = context.Response.StatusCode;
                    resposta.Mensagem = "Ocorreu um erro interno no servidor. Contate o suporte.";
                    break;
            }

            resposta.Timestamp = DateTime.UtcNow;
            resposta.Path = context.Request.Path;

            var json = JsonSerializer.Serialize(resposta, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    public class ErroResposta
    {
        public int StatusCode { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
