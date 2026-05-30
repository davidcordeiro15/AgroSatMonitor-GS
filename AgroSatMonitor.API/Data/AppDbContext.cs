using AgroSatMonitor.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroSatMonitor.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Fazenda> Fazendas { get; set; }
        public DbSet<CulturaAgricola> Culturas { get; set; }
        public DbSet<MonitoramentoClimatico> MonitoramentosClimaticos { get; set; }
        public DbSet<MonitoramentoVegetacao> MonitoramentosVegetacao { get; set; }
        public DbSet<AlertaAgricola> Alertas { get; set; }
        public DbSet<HistoricoConsulta> HistoricosConsulta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── HIERARQUIA DE MONITORAMENTO ───────────────────────────────────────────
            // HasBaseType(null) instrui o EF Core a tratar cada entidade concreta como
            // entidade raiz independente, evitando queries TPC com UNION ALL que causam
            // ORA-00904 no Oracle pré-23c. A herança C# permanece intacta para POO.
            modelBuilder.Entity<MonitoramentoClimatico>().HasBaseType((Type?)null);
            modelBuilder.Entity<MonitoramentoVegetacao>().HasBaseType((Type?)null);

            // ── FAZENDA ───────────────────────────────────────────────────────────────
            modelBuilder.Entity<Fazenda>(entity =>
            {
                entity.ToTable("TB_FAZENDA");
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id).HasColumnName("ID_FAZENDA").ValueGeneratedOnAdd();
                entity.Property(f => f.Nome).HasColumnName("NM_FAZENDA").HasMaxLength(200).IsRequired();
                entity.Property(f => f.Latitude).HasColumnName("NR_LATITUDE").IsRequired();
                entity.Property(f => f.Longitude).HasColumnName("NR_LONGITUDE").IsRequired();
                entity.Property(f => f.AreaHectares).HasColumnName("NR_AREA_HECTARES");
                entity.Property(f => f.Cidade).HasColumnName("NM_CIDADE").HasMaxLength(100).IsRequired();
                entity.Property(f => f.Estado).HasColumnName("SG_ESTADO").HasMaxLength(2).IsRequired();
                entity.Property(f => f.DataCadastro).HasColumnName("DT_CADASTRO").IsRequired();

                entity.HasMany(f => f.Culturas)
                      .WithOne(c => c.Fazenda)
                      .HasForeignKey(c => c.FazendaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(f => f.MonitoramentosClimaticos)
                      .WithOne(m => m.Fazenda)
                      .HasForeignKey(m => m.FazendaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(f => f.MonitoramentosVegetacao)
                      .WithOne(m => m.Fazenda)
                      .HasForeignKey(m => m.FazendaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(f => f.Alertas)
                      .WithOne(a => a.Fazenda)
                      .HasForeignKey(a => a.FazendaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(f => f.HistoricosConsulta)
                      .WithOne(h => h.Fazenda)
                      .HasForeignKey(h => h.FazendaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── CULTURA AGRICOLA ──────────────────────────────────────────────────────
            modelBuilder.Entity<CulturaAgricola>(entity =>
            {
                entity.ToTable("TB_CULTURA_AGRICOLA");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasColumnName("ID_CULTURA").ValueGeneratedOnAdd();
                entity.Property(c => c.Nome).HasColumnName("NM_CULTURA").HasMaxLength(100).IsRequired();
                entity.Property(c => c.Tipo).HasColumnName("TP_CULTURA").HasMaxLength(100).IsRequired();
                entity.Property(c => c.Safra).HasColumnName("DS_SAFRA").HasMaxLength(20).IsRequired();
                entity.Property(c => c.FazendaId).HasColumnName("ID_FAZENDA").IsRequired();
            });

            // ── MONITORAMENTO CLIMATICO ───────────────────────────────────────────────
            modelBuilder.Entity<MonitoramentoClimatico>(entity =>
            {
                entity.ToTable("TB_MON_CLIMATICO");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID_MON_CLI").ValueGeneratedOnAdd();
                entity.Property(e => e.FazendaId).HasColumnName("ID_FAZENDA");
                entity.Property(e => e.Latitude).HasColumnName("NR_LATITUDE");
                entity.Property(e => e.Longitude).HasColumnName("NR_LONGITUDE");
                entity.Property(e => e.DataCriacao).HasColumnName("DT_CRIACAO");
                entity.Property(e => e.Temperatura).HasColumnName("NR_TEMPERATURA");
                entity.Property(e => e.Umidade).HasColumnName("NR_UMIDADE");
                entity.Property(e => e.Precipitacao).HasColumnName("NR_PRECIPITACAO");
                entity.Property(e => e.VelocidadeVento).HasColumnName("NR_VEL_VENTO");
                entity.Property(e => e.DataLeitura).HasColumnName("DT_LEITURA");

                entity.HasOne(e => e.Fazenda)
                      .WithMany(f => f.MonitoramentosClimaticos)
                      .HasForeignKey(e => e.FazendaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── MONITORAMENTO VEGETACAO ───────────────────────────────────────────────
            modelBuilder.Entity<MonitoramentoVegetacao>(entity =>
            {
                entity.ToTable("TB_MON_VEGETACAO");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID_MON_VEG").ValueGeneratedOnAdd();
                entity.Property(e => e.FazendaId).HasColumnName("ID_FAZENDA");
                entity.Property(e => e.Latitude).HasColumnName("NR_LATITUDE");
                entity.Property(e => e.Longitude).HasColumnName("NR_LONGITUDE");
                entity.Property(e => e.DataCriacao).HasColumnName("DT_CRIACAO");
                entity.Property(e => e.Ndvi).HasColumnName("NR_NDVI");
                entity.Property(e => e.NivelSaudeVegetacao).HasColumnName("TP_NIVEL_SAUDE").HasConversion<int>();
                entity.Property(e => e.DataLeitura).HasColumnName("DT_LEITURA");

                entity.HasOne(e => e.Fazenda)
                      .WithMany(f => f.MonitoramentosVegetacao)
                      .HasForeignKey(e => e.FazendaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── ALERTA AGRICOLA ───────────────────────────────────────────────────────
            modelBuilder.Entity<AlertaAgricola>(entity =>
            {
                entity.ToTable("TB_ALERTA_AGRICOLA");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).HasColumnName("ID_ALERTA").ValueGeneratedOnAdd();
                entity.Property(a => a.Tipo).HasColumnName("TP_ALERTA").HasConversion<int>();
                entity.Property(a => a.Descricao).HasColumnName("DS_ALERTA").HasMaxLength(500);
                entity.Property(a => a.NivelRisco).HasColumnName("TP_NIVEL_RISCO").HasConversion<int>();
                entity.Property(a => a.DataGeracao).HasColumnName("DT_GERACAO");
                entity.Property(a => a.FazendaId).HasColumnName("ID_FAZENDA");
            });

            // ── HISTORICO CONSULTA ────────────────────────────────────────────────────
            modelBuilder.Entity<HistoricoConsulta>(entity =>
            {
                entity.ToTable("TB_HISTORICO_CONSULTA");
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Id).HasColumnName("ID_HISTORICO").ValueGeneratedOnAdd();
                entity.Property(h => h.EndpointConsultado).HasColumnName("DS_ENDPOINT").HasMaxLength(300);
                entity.Property(h => h.DataConsulta).HasColumnName("DT_CONSULTA");
                entity.Property(h => h.TempoRespostaMs).HasColumnName("NR_TEMPO_RESP_MS");
                entity.Property(h => h.Sucesso).HasColumnName("FL_SUCESSO").HasConversion<int>();
                entity.Property(h => h.FazendaId).HasColumnName("ID_FAZENDA");
            });
        }
    }
}