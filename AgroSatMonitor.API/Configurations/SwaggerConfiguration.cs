using Microsoft.OpenApi.Models;

namespace AgroSatMonitor.API.Configurations
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AgroSatMonitor API",
                    Version = "v1",
                    Description = """
                        API de monitoramento agrícola com dados de satélite e clima.
                        
                        Funcionalidades:
                        - Cadastro e gestão de fazendas
                        - Cadastro de culturas agrícolas
                        - Consulta de dados climáticos em tempo real (Open-Meteo API)
                        - Cálculo de índice de vegetação NDVI
                        - Geração automática de alertas agrícolas
                        - Histórico de consultas e monitoramentos
                        
                        
                        """,
                    Contact = new OpenApiContact
                    {
                        Name = "Equipe AgroSatMonitor",
                        Email = "agrosatmonitor@fiap.com.br"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
                options.DocInclusionPredicate((docName, api) => true);

                // Habilita comentários XML se existirem
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "AgroSatMonitor API v1");
                options.RoutePrefix = string.Empty; // Swagger na raiz
                options.DocumentTitle = "AgroSatMonitor API";
                options.DisplayRequestDuration();
            });

            return app;
        }
    }
}
