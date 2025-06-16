using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    [Table("Credenciales")]
    public class Credencial
    {
        [Key]
        [ForeignKey(nameof(Usuario))]
        public int IdUsuario { get; set; }

        [Required]
        public string PasswordEnc { get; set; } = null!;

        public Usuario Usuario { get; set; } = null!;
    }
}
