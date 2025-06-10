using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        // clave cifrada con AES-256 y en Base64
        [Required]
        public string PasswordEnc { get; set; } = null!;
    }
}
