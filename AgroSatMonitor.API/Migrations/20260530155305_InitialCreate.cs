using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroSatMonitor.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "MonitoramentoBaseSequence");

            migrationBuilder.CreateTable(
                name: "TB_FAZENDA",
                columns: table => new
                {
                    ID_FAZENDA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_FAZENDA = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    NR_LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    NR_LONGITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    NR_AREA_HECTARES = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    NM_CIDADE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    SG_ESTADO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    DT_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_FAZENDA", x => x.ID_FAZENDA);
                });

            migrationBuilder.CreateTable(
                name: "TB_ALERTA_AGRICOLA",
                columns: table => new
                {
                    ID_ALERTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TP_ALERTA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DS_ALERTA = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    TP_NIVEL_RISCO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DT_GERACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ID_FAZENDA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_ALERTA_AGRICOLA", x => x.ID_ALERTA);
                    table.ForeignKey(
                        name: "FK_TB_ALERTA_AGRICOLA_TB_FAZENDA_ID_FAZENDA",
                        column: x => x.ID_FAZENDA,
                        principalTable: "TB_FAZENDA",
                        principalColumn: "ID_FAZENDA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_CULTURA_AGRICOLA",
                columns: table => new
                {
                    ID_CULTURA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_CULTURA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TP_CULTURA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DS_SAFRA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    ID_FAZENDA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CULTURA_AGRICOLA", x => x.ID_CULTURA);
                    table.ForeignKey(
                        name: "FK_TB_CULTURA_AGRICOLA_TB_FAZENDA_ID_FAZENDA",
                        column: x => x.ID_FAZENDA,
                        principalTable: "TB_FAZENDA",
                        principalColumn: "ID_FAZENDA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_HISTORICO_CONSULTA",
                columns: table => new
                {
                    ID_HISTORICO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DS_ENDPOINT = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    DT_CONSULTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    NR_TEMPO_RESP_MS = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    FL_SUCESSO = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    ID_FAZENDA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_HISTORICO_CONSULTA", x => x.ID_HISTORICO);
                    table.ForeignKey(
                        name: "FK_TB_HISTORICO_CONSULTA_TB_FAZENDA_ID_FAZENDA",
                        column: x => x.ID_FAZENDA,
                        principalTable: "TB_FAZENDA",
                        principalColumn: "ID_FAZENDA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_MON_CLIMATICO",
                columns: table => new
                {
                    ID_MON_VEG = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "\"MonitoramentoBaseSequence\".NEXTVAL"),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    NR_LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    NR_LONGITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    ID_FAZENDA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NR_TEMPERATURA = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    NR_UMIDADE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    NR_PRECIPITACAO = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    NR_VEL_VENTO = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    DT_LEITURA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_MON_CLIMATICO", x => x.ID_MON_VEG);
                    table.ForeignKey(
                        name: "FK_TB_MON_CLIMATICO_TB_FAZENDA_ID_FAZENDA",
                        column: x => x.ID_FAZENDA,
                        principalTable: "TB_FAZENDA",
                        principalColumn: "ID_FAZENDA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_MON_VEGETACAO",
                columns: table => new
                {
                    ID_MON_VEG = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "\"MonitoramentoBaseSequence\".NEXTVAL"),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    NR_LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    NR_LONGITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    ID_FAZENDA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NR_NDVI = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    TP_NIVEL_SAUDE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DT_LEITURA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_MON_VEGETACAO", x => x.ID_MON_VEG);
                    table.ForeignKey(
                        name: "FK_TB_MON_VEGETACAO_TB_FAZENDA_ID_FAZENDA",
                        column: x => x.ID_FAZENDA,
                        principalTable: "TB_FAZENDA",
                        principalColumn: "ID_FAZENDA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_ALERTA_AGRICOLA_ID_FAZENDA",
                table: "TB_ALERTA_AGRICOLA",
                column: "ID_FAZENDA");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CULTURA_AGRICOLA_ID_FAZENDA",
                table: "TB_CULTURA_AGRICOLA",
                column: "ID_FAZENDA");

            migrationBuilder.CreateIndex(
                name: "IX_TB_HISTORICO_CONSULTA_ID_FAZENDA",
                table: "TB_HISTORICO_CONSULTA",
                column: "ID_FAZENDA");

            migrationBuilder.CreateIndex(
                name: "IX_TB_MON_CLIMATICO_ID_FAZENDA",
                table: "TB_MON_CLIMATICO",
                column: "ID_FAZENDA");

            migrationBuilder.CreateIndex(
                name: "IX_TB_MON_VEGETACAO_ID_FAZENDA",
                table: "TB_MON_VEGETACAO",
                column: "ID_FAZENDA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_ALERTA_AGRICOLA");

            migrationBuilder.DropTable(
                name: "TB_CULTURA_AGRICOLA");

            migrationBuilder.DropTable(
                name: "TB_HISTORICO_CONSULTA");

            migrationBuilder.DropTable(
                name: "TB_MON_CLIMATICO");

            migrationBuilder.DropTable(
                name: "TB_MON_VEGETACAO");

            migrationBuilder.DropTable(
                name: "TB_FAZENDA");

            migrationBuilder.DropSequence(
                name: "MonitoramentoBaseSequence");
        }
    }
}
