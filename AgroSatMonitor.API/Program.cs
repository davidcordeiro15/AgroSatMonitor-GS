using AgroSatMonitor.API.Configurations;
using AgroSatMonitor.API.Data;
using AgroSatMonitor.API.Exceptions;
using AgroSatMonitor.API.ExternalServices;
using AgroSatMonitor.API.Interfaces;
using AgroSatMonitor.API.Repositories;
using AgroSatMonitor.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── CONTROLLERS ───────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ── SWAGGER ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// ── ORACLE + ENTITY FRAMEWORK CORE ───────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// ── HTTP CLIENTS (External Services) ─────────────────────────────────────────
builder.Services.AddHttpClient<ClimaApiClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.Add("User-Agent", "AgroSatMonitor/1.0");
});

builder.Services.AddHttpClient<VegetacaoApiClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.Add("User-Agent", "AgroSatMonitor/1.0");
});

// ── REPOSITORIES ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IFazendaRepository, FazendaRepository>();

// ── SERVICES ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IFazendaService, FazendaService>();
builder.Services.AddScoped<IClimaService, ClimaService>();
builder.Services.AddScoped<IVegetacaoService, VegetacaoService>();
builder.Services.AddScoped<IMonitoramentoService, MonitoramentoService>();
builder.Services.AddScoped<ICulturaAgricolaService, CulturaAgricolaService>();

// ── LOGGING ───────────────────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// ── MIDDLEWARE ────────────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
