namespace AuthService.Infrastructure.Entities
{
    public class UsuarioEntity
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
    }
}
