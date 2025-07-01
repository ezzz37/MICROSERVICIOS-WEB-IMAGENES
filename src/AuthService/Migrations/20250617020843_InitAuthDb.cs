using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthService.Migrations
{
    public partial class InitAuthDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Master Key
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.symmetric_keys WHERE name = '##MS_DatabaseMasterKey##')
    CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'TuPasswordMaestroSegura!';
");

            // 2) Certificado
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.certificates WHERE name = 'CertUsuarios')
    CREATE CERTIFICATE CertUsuarios
        WITH SUBJECT = 'Certificado para proteger la llave simétrica de Usuarios';
");

            // 3) Symmetric Key
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.symmetric_keys WHERE name = 'KeyUsuarios')
    CREATE SYMMETRIC KEY KeyUsuarios
        WITH ALGORITHM = AES_256
        ENCRYPTION BY CERTIFICATE CertUsuarios;
");

            // 4) Semilla de usuario '1'/'1'
            migrationBuilder.Sql(@"
OPEN SYMMETRIC KEY KeyUsuarios
    DECRYPTION BY CERTIFICATE CertUsuarios;

INSERT INTO dbo.Usuarios (Username, PasswordEnc)
VALUES (
    '1',
    EncryptByKey(
        KEY_GUID('KeyUsuarios'),
        CONVERT(VARBINARY(MAX), '1')
    )
);

CLOSE SYMMETRIC KEY KeyUsuarios;
");

            // 5a) Eliminar SP si existe (batch separado)
            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.sp_ValidarUsuario','P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ValidarUsuario;
");

            // 5b) Crear SP en su propio batch
            migrationBuilder.Sql(@"
CREATE PROCEDURE dbo.sp_ValidarUsuario
    @Username      NVARCHAR(50),
    @PasswordInput NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    OPEN SYMMETRIC KEY KeyUsuarios 
        DECRYPTION BY CERTIFICATE CertUsuarios;

    SELECT IdUsuario
      FROM dbo.Usuarios
     WHERE Username = @Username
       AND CONVERT(NVARCHAR(MAX), DecryptByKey(PasswordEnc)) = @PasswordInput;

    CLOSE SYMMETRIC KEY KeyUsuarios;
END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar SP
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_ValidarUsuario;");

            // Eliminar objetos criptográficos
            migrationBuilder.Sql("DROP SYMMETRIC KEY IF EXISTS KeyUsuarios;");
            migrationBuilder.Sql("DROP CERTIFICATE IF EXISTS CertUsuarios;");
            migrationBuilder.Sql("DROP MASTER KEY IF EXISTS;");
        }
    }
}
