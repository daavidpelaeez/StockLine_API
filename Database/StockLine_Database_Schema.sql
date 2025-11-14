-- =============================================
-- StockLine Database Schema Script
-- Generado: 2024
-- Descripción: Script completo de creación de la base de datos StockLine
-- =============================================

USE master;
GO

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'StockLine')
BEGIN
    CREATE DATABASE StockLine;
END
GO

USE StockLine;
GO

-- =============================================
-- ELIMINAR TABLAS EXISTENTES (en orden correcto por dependencias)
-- =============================================
IF OBJECT_ID('dbo.EnviosDetalle', 'U') IS NOT NULL DROP TABLE dbo.EnviosDetalle;
IF OBJECT_ID('dbo.MovimientosStock', 'U') IS NOT NULL DROP TABLE dbo.MovimientosStock;
IF OBJECT_ID('dbo.Envios', 'U') IS NOT NULL DROP TABLE dbo.Envios;
IF OBJECT_ID('dbo.SIMs', 'U') IS NOT NULL DROP TABLE dbo.SIMs;
IF OBJECT_ID('dbo.ComercialAyuntamiento', 'U') IS NOT NULL DROP TABLE dbo.ComercialAyuntamiento;
IF OBJECT_ID('dbo.Ayuntamientos', 'U') IS NOT NULL DROP TABLE dbo.Ayuntamientos;
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
IF OBJECT_ID('dbo.Productos', 'U') IS NOT NULL DROP TABLE dbo.Productos;
IF OBJECT_ID('dbo.Categorias', 'U') IS NOT NULL DROP TABLE dbo.Categorias;
IF OBJECT_ID('dbo.Comerciales', 'U') IS NOT NULL DROP TABLE dbo.Comerciales;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

-- =============================================
-- TABLA: Roles
-- =============================================
CREATE TABLE dbo.Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(255) NULL
);
GO

-- =============================================
-- TABLA: Usuarios
-- =============================================
CREATE TABLE dbo.Usuarios (
    UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RoleID) REFERENCES dbo.Roles(RoleID)
);
GO

-- =============================================
-- TABLA: Comerciales
-- =============================================
CREATE TABLE dbo.Comerciales (
    ComercialID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Telefono NVARCHAR(20) NULL
);
GO

-- =============================================
-- TABLA: Ayuntamientos
-- =============================================
CREATE TABLE dbo.Ayuntamientos (
    AyuntamientoID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    Direccion NVARCHAR(255) NULL,
    Ciudad NVARCHAR(100) NULL,
    Provincia NVARCHAR(100) NULL,
    CP NVARCHAR(10) NULL,
    Telefono NVARCHAR(20) NULL,
    CreatedAt DATETIME NULL DEFAULT GETDATE(),
    Email NVARCHAR(255) NULL,
    ComercialID INT NULL,
    CONSTRAINT FK_Ayuntamientos_Comerciales FOREIGN KEY (ComercialID) REFERENCES dbo.Comerciales(ComercialID)
);
GO

-- =============================================
-- TABLA: Categorias
-- =============================================
CREATE TABLE dbo.Categorias (
    CategoriaID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- =============================================
-- TABLA: Productos
-- =============================================
CREATE TABLE dbo.Productos (
    ProductoID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(MAX) NULL,
    Stock INT NOT NULL DEFAULT 0,
    Foto NVARCHAR(MAX) NULL,
    CategoriaID INT NULL,
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaID) REFERENCES dbo.Categorias(CategoriaID)
);
GO

-- =============================================
-- TABLA: SIMs
-- =============================================
CREATE TABLE dbo.SIMs (
    SIMID INT IDENTITY(1,1) PRIMARY KEY,
    NumeroSIM NVARCHAR(50) NOT NULL UNIQUE,
    ProductoID INT NULL,
    Ubicacion NVARCHAR(255) NULL,
    Estado NVARCHAR(50) NULL,
    FechaAsignacion DATETIME NULL,
    CONSTRAINT FK_SIMs_Productos FOREIGN KEY (ProductoID) REFERENCES dbo.Productos(ProductoID)
);
GO

-- =============================================
-- TABLA: Envios
-- =============================================
CREATE TABLE dbo.Envios (
    EnvioID INT IDENTITY(1,1) PRIMARY KEY,
    AyuntamientoID INT NOT NULL,
    ComercialID INT NOT NULL,
    NumeroReferencia NVARCHAR(50) NOT NULL,
    Estado NVARCHAR(20) NOT NULL CHECK (Estado IN ('Pendiente', 'Preparado', 'Enviado', 'Archivado')),
    FechaEnvio DATETIME NOT NULL DEFAULT GETDATE(),
    UsuarioModificadorID INT NULL,
    FechaModificacion DATETIME NULL,
    Ubicacion NVARCHAR(255) NULL,
    CONSTRAINT FK_Envios_Ayuntamientos FOREIGN KEY (AyuntamientoID) REFERENCES dbo.Ayuntamientos(AyuntamientoID),
    CONSTRAINT FK_Envios_Comerciales FOREIGN KEY (ComercialID) REFERENCES dbo.Comerciales(ComercialID),
    CONSTRAINT FK_Envios_Usuarios FOREIGN KEY (UsuarioModificadorID) REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- =============================================
-- TABLA: EnviosDetalle
-- =============================================
CREATE TABLE dbo.EnviosDetalle (
    EnvioDetalleID INT IDENTITY(1,1) PRIMARY KEY,
    EnvioID INT NOT NULL,
    ProductoID INT NULL, -- Ahora permite nulos
    Cantidad INT NOT NULL,
    SIMID INT NULL,
    CONSTRAINT FK_EnviosDetalle_Envios FOREIGN KEY (EnvioID) REFERENCES dbo.Envios(EnvioID),
    -- El constraint de ProductoID se crea después para ON DELETE SET NULL
    CONSTRAINT FK_EnviosDetalle_SIMs FOREIGN KEY (SIMID) REFERENCES dbo.SIMs(SIMID)
);
GO

-- Añadir constraint de ProductoID con ON DELETE SET NULL
ALTER TABLE dbo.EnviosDetalle
ADD CONSTRAINT FK_EnviosDetalle_Productos FOREIGN KEY (ProductoID) REFERENCES dbo.Productos(ProductoID) ON DELETE SET NULL;
GO

-- =============================================
-- TABLA: MovimientosStock
-- =============================================
CREATE TABLE dbo.MovimientosStock (
    MovimientoID INT IDENTITY(1,1) PRIMARY KEY,
    ProductoID INT NULL, -- Permitir nulos para ON DELETE SET NULL
    Cantidad INT NOT NULL,
    TipoMovimiento NVARCHAR(20) NOT NULL CHECK (TipoMovimiento IN ('Entrada', 'Salida')),
    UsuarioID INT NOT NULL,
    Observaciones NVARCHAR(MAX) NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    -- El constraint de ProductoID se crea después para ON DELETE SET NULL
    CONSTRAINT FK_MovimientosStock_Usuarios FOREIGN KEY (UsuarioID) REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- Añadir constraint de ProductoID con ON DELETE SET NULL
ALTER TABLE dbo.MovimientosStock
ADD CONSTRAINT FK_MovimientosStock_Productos FOREIGN KEY (ProductoID) REFERENCES dbo.Productos(ProductoID) ON DELETE SET NULL;
GO

-- =============================================
-- TABLA: ComercialAyuntamiento (si existe relación muchos a muchos)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ComercialAyuntamiento')
BEGIN
    CREATE TABLE dbo.ComercialAyuntamiento (
        ComercialID INT NOT NULL,
        AyuntamientoID INT NOT NULL,
        PRIMARY KEY (ComercialID, AyuntamientoID),
        CONSTRAINT FK_ComercialAyuntamiento_Comerciales FOREIGN KEY (ComercialID) REFERENCES dbo.Comerciales(ComercialID),
        CONSTRAINT FK_ComercialAyuntamiento_Ayuntamientos FOREIGN KEY (AyuntamientoID) REFERENCES dbo.Ayuntamientos(AyuntamientoID)
    );
END
GO

-- =============================================
-- DATOS INICIALES: Roles
-- =============================================
SET IDENTITY_INSERT dbo.Roles ON;
INSERT INTO dbo.Roles (RoleID, Nombre, Descripcion) VALUES
(1, 'Administrador', 'Acceso completo al sistema'),
(2, 'Usuario', 'Acceso limitado al sistema'),
(3, 'Comercial', 'Acceso para comerciales');
SET IDENTITY_INSERT dbo.Roles OFF;
GO

-- =============================================
-- DATOS INICIALES: Usuarios (password: "password123" hasheado)
-- =============================================
SET IDENTITY_INSERT dbo.Usuarios ON;
INSERT INTO dbo.Usuarios (UsuarioID, Nombre, Apellidos, Email, PasswordHash, RoleID) VALUES
(1, 'Admin', 'Sistema', 'admin@stockline.com', 'AQAAAAEAACcQAAAAEJ1234567890ABCDEFGHIJKLMNOP', 1);
SET IDENTITY_INSERT dbo.Usuarios OFF;
GO

-- =============================================
-- ÍNDICES PARA MEJORAR RENDIMIENTO
-- =============================================
CREATE INDEX IX_Usuarios_Email ON dbo.Usuarios(Email);
CREATE INDEX IX_Usuarios_RoleID ON dbo.Usuarios(RoleID);
CREATE INDEX IX_Ayuntamientos_ComercialID ON dbo.Ayuntamientos(ComercialID);
CREATE INDEX IX_Productos_CategoriaID ON dbo.Productos(CategoriaID);
CREATE INDEX IX_SIMs_ProductoID ON dbo.SIMs(ProductoID);
CREATE INDEX IX_Envios_AyuntamientoID ON dbo.Envios(AyuntamientoID);
CREATE INDEX IX_Envios_ComercialID ON dbo.Envios(ComercialID);
CREATE INDEX IX_Envios_Estado ON dbo.Envios(Estado);
CREATE INDEX IX_EnviosDetalle_EnvioID ON dbo.EnviosDetalle(EnvioID);
CREATE INDEX IX_EnviosDetalle_ProductoID ON dbo.EnviosDetalle(ProductoID);
CREATE INDEX IX_MovimientosStock_ProductoID ON dbo.MovimientosStock(ProductoID);
CREATE INDEX IX_MovimientosStock_UsuarioID ON dbo.MovimientosStock(UsuarioID);
CREATE INDEX IX_MovimientosStock_Fecha ON dbo.MovimientosStock(Fecha);
GO

PRINT 'Base de datos StockLine creada exitosamente.';
GO
