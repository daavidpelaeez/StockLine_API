-- =============================================
-- StockLine - Datos de Ejemplo (actualizado)
-- =============================================

USE StockLine;
GO

-- Comerciales
SET IDENTITY_INSERT dbo.Comerciales ON;
INSERT INTO dbo.Comerciales (ComercialID, Nombre, Apellidos, Email, Telefono) VALUES
(1, 'Juan', 'García López', 'juan.garcia@example.com', '600111222'),
(2, 'María', 'Martínez Sánchez', 'maria.martinez@example.com', '600333444'),
(3, 'Pedro', 'Rodríguez Pérez', 'pedro.rodriguez@example.com', '600555666');
SET IDENTITY_INSERT dbo.Comerciales OFF;
GO

-- Ayuntamientos
SET IDENTITY_INSERT dbo.Ayuntamientos ON;
INSERT INTO dbo.Ayuntamientos (AyuntamientoID, Nombre, Direccion, Ciudad, Provincia, CP, Telefono, Email, ComercialID, CreatedAt, Activo) VALUES
(1, 'Ayuntamiento de Madrid', 'Plaza de la Villa, 4', 'Madrid', 'Madrid', '28005', '915881000', 'info@madrid.es', 1, GETDATE(), 1),
(2, 'Ayuntamiento de Barcelona', 'Plaça de Sant Jaume, 1', 'Barcelona', 'Barcelona', '08002', '932912012', 'info@barcelona.cat', 1, GETDATE(), 1),
(3, 'Ayuntamiento de Valencia', 'Plaza del Ayuntamiento, 1', 'Valencia', 'Valencia', '46002', '963525478', 'info@valencia.es', 2, GETDATE(), 1),
(4, 'Ayuntamiento de Sevilla', 'Plaza Nueva, 1', 'Sevilla', 'Sevilla', '41001', '954590000', 'info@sevilla.org', 2, GETDATE(), 1),
(5, 'Ayuntamiento de Zaragoza', 'Plaza del Pilar, 18', 'Zaragoza', 'Zaragoza', '50003', '976721100', 'info@zaragoza.es', 3, GETDATE(), 1);
SET IDENTITY_INSERT dbo.Ayuntamientos OFF;
GO

-- Categorias
SET IDENTITY_INSERT dbo.Categorias ON;
INSERT INTO dbo.Categorias (CategoriaID, Nombre, Activo) VALUES
(1, 'Routers', 1),
(2, 'Switches', 1),
(3, 'Access Points', 1),
(4, 'Tarjetas SIM', 1),
(5, 'Cables y Accesorios', 1),
(6, 'Equipos de Videoconferencia', 1);
SET IDENTITY_INSERT dbo.Categorias OFF;
GO

-- Productos
SET IDENTITY_INSERT dbo.Productos ON;
INSERT INTO dbo.Productos (ProductoID, Nombre, Descripcion, Stock, Foto, CategoriaID, Activo) VALUES
(1, 'Router TP-Link AC1200', 'Router Dual Band AC1200 Gigabit', 50, NULL, 1, 1),
(2, 'Switch Cisco 24 puertos', 'Switch managed 24 puertos Gigabit', 30, NULL, 2, 1),
(3, 'Access Point Ubiquiti UAP-AC-LR', 'Access Point de largo alcance', 25, NULL, 3, 1),
(4, 'Tarjeta SIM Movistar', 'Tarjeta SIM datos 50GB', 100, NULL, 4, 1),
(5, 'Tarjeta SIM Vodafone', 'Tarjeta SIM datos 50GB', 100, NULL, 4, 1),
(6, 'Cable Ethernet Cat6 5m', 'Cable de red Cat6 5 metros', 200, NULL, 5, 1),
(7, 'Webcam Logitech C920', 'Cámara web HD 1080p', 40, NULL, 6, 1),
(8, 'Router 4G Huawei B525', 'Router 4G LTE Cat6', 35, NULL, 1, 1);
SET IDENTITY_INSERT dbo.Productos OFF;
GO

-- SIMs
SET IDENTITY_INSERT dbo.SIMs ON;
INSERT INTO dbo.SIMs (SIMID, NumeroSIM, ProductoID, FechaAsignacion, Ubicacion, Estado) VALUES
(1, '8934071234567890123', 4, NULL, 'En almacén', 'Disponible'),
(2, '8934071234567890124', 4, NULL, 'En almacén', 'Disponible'),
(3, '8934071234567890125', 4, NULL, 'En almacén', 'Disponible'),
(4, '8934061234567890123', 5, NULL, 'En almacén', 'Disponible'),
(5, '8934061234567890124', 5, NULL, 'En almacén', 'Disponible'),
(6, '8934061234567890125', 5, NULL, 'En almacén', 'Disponible');
SET IDENTITY_INSERT dbo.SIMs OFF;
GO

-- Roles
SET IDENTITY_INSERT dbo.Roles ON;
INSERT INTO dbo.Roles (RoleID, Nombre, Descripcion) VALUES
(1, 'Administrador', 'Acceso completo al sistema'),
(2, 'Usuario', 'Acceso limitado al sistema'),
(3, 'Comercial', 'Acceso para comerciales');
SET IDENTITY_INSERT dbo.Roles OFF;
GO

-- Usuarios
SET IDENTITY_INSERT dbo.Usuarios ON;
INSERT INTO dbo.Usuarios (UsuarioID, Nombre, Apellidos, Email, PasswordHash, RoleID, Activo) VALUES
(1, 'Admin', 'Sistema', 'admin@stockline.com', 'AQAAAAEAACcQAAAAEJ1234567890ABCDEFGHIJKLMNOP', 1, 1),
(2, 'Luis', 'Fernández', 'luis.fernandez@stockline.com', 'AQAAAAEAACcQAAAAEJ1234567890ABCDEFGHIJKLMNOP', 2, 1),
(3, 'Ana', 'González', 'ana.gonzalez@stockline.com', 'AQAAAAEAACcQAAAAEJ1234567890ABCDEFGHIJKLMNOP', 2, 1),
(4, 'Carlos', 'Ruiz', 'carlos.ruiz@stockline.com', 'AQAAAAEAACcQAAAAEJ1234567890ABCDEFGHIJKLMNOP', 3, 1);
SET IDENTITY_INSERT dbo.Usuarios OFF;
GO

-- Envios
SET IDENTITY_INSERT dbo.Envios ON;
INSERT INTO dbo.Envios (EnvioID, AyuntamientoID, ComercialID, NumeroReferencia, Estado, FechaEnvio, UsuarioModificadorID, FechaModificacion, Ubicacion) VALUES
(1, 1, 1, 'ENV-2024-001', 'Enviado', DATEADD(day, -10, GETDATE()), 1, NULL, 'Ayuntamiento de Madrid'),
(2, 2, 1, 'ENV-2024-002', 'Preparado', DATEADD(day, -5, GETDATE()), 1, NULL, 'Ayuntamiento de Barcelona'),
(3, 3, 2, 'ENV-2024-003', 'Pendiente', DATEADD(day, -2, GETDATE()), 2, NULL, 'Ayuntamiento de Valencia'),
(4, 4, 2, 'ENV-2024-004', 'Archivado', DATEADD(day, -1, GETDATE()), 2, NULL, 'Ayuntamiento de Sevilla');
SET IDENTITY_INSERT dbo.Envios OFF;
GO

-- EnviosDetalle
SET IDENTITY_INSERT dbo.EnviosDetalle ON;
INSERT INTO dbo.EnviosDetalle (EnvioDetalleID, EnvioID, ProductoID, Cantidad, SIMID) VALUES
(1, 1, 1, 5, NULL),
(2, 1, 4, 10, 1),
(3, 1, 6, 20, NULL),
(4, 2, 2, 3, NULL),
(5, 2, 5, 15, 4),
(6, 3, 3, 8, NULL),
(7, 3, 7, 5, NULL);
SET IDENTITY_INSERT dbo.EnviosDetalle OFF;
GO

-- MovimientosStock
SET IDENTITY_INSERT dbo.MovimientosStock ON;
INSERT INTO dbo.MovimientosStock (MovimientoID, ProductoID, Cantidad, TipoMovimiento, UsuarioID, Observaciones, Fecha) VALUES
(1, 1, 50, 'Entrada', 1, 'Entrada inicial de stock', DATEADD(day, -30, GETDATE())),
(2, 2, 30, 'Entrada', 1, 'Entrada inicial de stock', DATEADD(day, -30, GETDATE())),
(3, 1, -5, 'Salida', 1, 'Envio 1 - ENV-2024-001', DATEADD(day, -10, GETDATE())),
(4, 4, -10, 'Salida', 1, 'Envio 1 - ENV-2024-001', DATEADD(day, -10, GETDATE())),
(5, 2, -3, 'Salida', 1, 'Envio 2 - ENV-2024-002', DATEADD(day, -5, GETDATE()));
SET IDENTITY_INSERT dbo.MovimientosStock OFF;
GO

PRINT 'Datos de ejemplo insertados correctamente.';
GO
