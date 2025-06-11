USE master;
GO

-- Si existe, eliminar la BD
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'AuthServiceBD')
BEGIN
    ALTER DATABASE [AuthServiceBD] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [AuthServiceBD];
END
GO

-- Crear la base de datos
CREATE DATABASE [AuthServiceBD];
GO

USE [AuthServiceBD];
GO

-- 10.a) Master Key
CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'TuPasswordMaestroSegura!';
GO

-- 10.b) Certificado
CREATE CERTIFICATE CertUsuarios
    WITH SUBJECT = 'Certificado para proteger la llave sim�trica de Usuarios';
GO

-- 10.c) Symmetric Key AES-256
CREATE SYMMETRIC KEY KeyUsuarios
    WITH ALGORITHM = AES_256
    ENCRYPTION BY CERTIFICATE CertUsuarios;
GO

-- Tabla Usuarios
CREATE TABLE dbo.Usuarios (
    IdUsuario    INT            IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50)   NOT NULL,
    PasswordEnc  VARBINARY(MAX) NOT NULL
);
GO

-- �ndice �nico en Username
CREATE UNIQUE INDEX UX_Usuarios_Username
    ON dbo.Usuarios (Username);
GO

-- Insertar usuario '1'/'1' cifrado
OPEN SYMMETRIC KEY KeyUsuarios
    DECRYPTION BY CERTIFICATE CertUsuarios;
GO

INSERT INTO dbo.Usuarios (Username, PasswordEnc)
VALUES (
    '1',
    EncryptByKey(
        KEY_GUID('KeyUsuarios'),
        CONVERT(VARBINARY(MAX), '1')
    )
);
GO

CLOSE SYMMETRIC KEY KeyUsuarios;
GO

-- Stored Procedure de validaci�n
CREATE PROCEDURE dbo.sp_ValidarUsuario
(
    @Username      NVARCHAR(50),
    @PasswordInput NVARCHAR(MAX)
)
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
END
GO
