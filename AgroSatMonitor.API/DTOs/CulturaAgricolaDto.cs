using System.ComponentModel.DataAnnotations;

namespace AgroSatMonitor.API.DTOs
{
    public class CulturaAgricolaRequestDto
    {
        [Required(ErrorMessage = "O nome da cultura é obrigatório.")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de cultura é obrigatório.")]
        [MaxLength(100)]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A safra é obrigatória.")]
        [MaxLength(20)]
        public string Safra { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID da fazenda é obrigatório.")]
        public int FazendaId { get; set; }
    }

    public class CulturaAgricolaResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Safra { get; set; } = string.Empty;
        public int FazendaId { get; set; }
        public string NomeFazenda { get; set; } = string.Empty;
    }
}
