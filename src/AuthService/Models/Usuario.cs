using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = null!;

        // Mapea tu VARBINARY(MAX) PasswordEnc
        [Required]
        public byte[] PasswordEnc { get; set; } = null!;
    }
}
