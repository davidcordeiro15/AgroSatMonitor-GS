using System.ComponentModel.DataAnnotations;

namespace AgroSatMonitor.API.DTOs
{
    public class FazendaRequestDto
    {
        [Required(ErrorMessage = "O nome da fazenda é obrigatório.")]
        [MaxLength(200, ErrorMessage = "O nome não pode exceder 200 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A latitude é obrigatória.")]
        [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90.")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "A longitude é obrigatória.")]
        [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180.")]
        public double Longitude { get; set; }

        [Range(0.1, 1000000, ErrorMessage = "Área deve ser maior que 0.")]
        public double AreaHectares { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [MaxLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado é obrigatório.")]
        [MaxLength(2, ErrorMessage = "Use a sigla do estado (ex: SP).")]
        public string Estado { get; set; } = string.Empty;
    }

    public class FazendaResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AreaHectares { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
    }
}
