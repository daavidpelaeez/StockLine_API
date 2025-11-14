-- =============================================
-- StockLine - Datos de Ejemplo
-- Descripción: Script para insertar datos de prueba
-- =============================================

USE StockLine;
GO

PRINT 'Insertando datos de ejemplo...';
GO

-- =============================================
-- DATOS DE EJEMPLO: Comerciales
-- =============================================
SET IDENTITY_INSERT dbo.Comerciales ON;
INSERT INTO dbo.Comerciales (ComercialID, Nombre, Apellidos, Email, Telefono) VALUES
(1, 'Juan', 'García López', 'juan.garcia@example.com', '600111222'),
(2, 'María', 'Martínez Sánchez', 'maria.martinez@example.com', '600333444'),
(3, 'Pedro', 'Rodríguez Pérez', 'pedro.rodriguez@example.com', '600555666');
SET IDENTITY_INSERT dbo.Comerciales OFF;
GO

-- =============================================
-- DATOS DE EJEMPLO: Ayuntamientos
-- =============================================
SET IDENTITY_INSERT dbo.Ayuntamientos ON;
INSERT INTO dbo.Ayuntamientos (AyuntamientoID, Nombre, Direccion, Ciudad, Provincia, CP, Telefono, Email, ComercialID) VALUES
(1, 'Ayuntamiento de Madrid', 'Plaza de la Villa, 4', 'Madrid', 'Madrid', '28005', '915881000', 'info@madrid.es', 1),
(2, 'Ayuntamiento de Barcelona', 'Plaça de Sant Jaume, 1', 'Barcelona', 'Barcelona', '08002', '932912012', 'info@barcelona.cat', 1),
(3, 'Ayuntamiento de Valencia', 'Plaza del Ayuntamiento, 1', 'Valencia', 'Valencia', '46002', '963525478', 'info@valencia.es', 2),
(4, 'Ayuntamiento de Sevilla', 'Plaza Nueva, 1', 'Sevilla', 'Sevilla', '41001', '954590000', 'info@sevilla.org', 2),
(5, 'Ayuntamiento de Zaragoza', 'Plaza del Pilar, 18', 'Zaragoza', 'Zaragoza', '50003', '976721100', 'info@zaragoza.es', 3);
SET IDENTITY_INSERT dbo.Ayuntamientos OFF;
GO

-- =============================================
-- DATOS DE EJEMPLO: Categorías
-- =============================================
SET IDENTITY_INSERT dbo.Categorias ON;
INSERT INTO dbo.Categorias (CategoriaID, Nombre) VALUES
(1, 'Routers'),
(2, 'Switches'),
(3, 'Access Points'),
(4, 'Tarjetas SIM'),
(5, 'Cables y Accesorios'),
(6, 'Equipos de Videoconferencia');
SET IDENTITY_INSERT dbo.Categorias OFF;
GO

-- =============================================
-- DATOS DE EJEMPLO: Productos
-- =============================================
SET IDENTITY_INSERT dbo.Productos ON;
INSERT INTO dbo.Productos (ProductoID, Nombre, Descripcion, Stock, CategoriaID) VALUES
(1, 'Router TP-Link AC1200', 'Router Dual Band AC1200 Gigabit', 50, 1),
(2, 'Switch Cisco 24 puertos', 'Switch managed 24 puertos Gigabit', 30, 2),
(3, 'Access Point Ubiquiti UAP-AC-LR', 'Access Point de largo alcance', 25, 3),
(4, 'Tarjeta SIM Movistar', 'Tarjeta SIM datos 50GB', 100, 4),
(5, 'Tarjeta SIM Vodafone', 'Tarjeta SIM datos 50GB', 100, 4),
(6, 'Cable Ethernet Cat6 5m', 'Cable de red Cat6 5 metros', 200, 5),
(7, 'Webcam Logitech C920', 'Cámara web HD 1080p', 40, 6),
(8, 'Router 4G Huawei B525', 'Router 4G LTE Cat6', 35, 1);
SET IDENTITY_INSERT dbo.Productos OFF;
GO

-- =============================================
-- DATOS DE EJEMPLO: SIMs
-- =============================================
SET IDENTITY_INSERT dbo.SIMs ON;
INSERT INTO dbo.SIMs (SIMID, NumeroSIM, ProductoID, Ubicacion, Estado, FechaAsignacion) VALUES
(1, '8934071234567890123', 4, 'En almacén', 'Disponible', NULL),
(2, '8934071234567890124', 4, 'En almacén', 'Disponible', NULL),
(3, '8934071234567890125', 4, 'En almacén', 'Disponible', NULL),
(4, '8934061234567890123', 5, 'En almacén', 'Disponible', NULL),
(5, '8934061234567890124', 5, 'En almacén', 'Disponible', NULL),
(6, '8934061234567890125', 5, 'En almacén', 'Disponible', NULL);
SET IDENTITY_INSERT dbo.SIMs OFF;
GO

-- =============================================
-- DATOS DE EJEMPLO: Usuarios adicionales
-- =============================================
SET IDENTITY_INSERT dbo.Usuarios ON;
INSERT INTO dbo.Usuarios (UsuarioID, Nombre, Apellidos, Email, PasswordHash, RoleID) VALUES
(2, 'Luis', 'Fernández', 'luis.fernandez@stockline.com', 'AQAAAAEAACcQAAAAEJ1234567890ABCDEFGHIJKLMNOP', 2),
(3, 'Ana', 'González', 'ana.gonzalez@stockline.com', 'AQAAAAEAACcQAAAAEJ1234567890ABCDEFGHIJKLMNOP', 2),
(4, 'Carlos', 'Ruiz', 'carlos.ruiz@stockline.com', 'AQAAAAEAACcQAAAAEJ1234567890ABCDEFGHIJKLMNOP', 3);
SET IDENTITY_INSERT dbo.Usuarios OFF;
GO

-- =============================================
-- DATOS DE EJEMPLO: Envíos
-- =============================================
SET IDENTITY_INSERT dbo.Envios ON;
INSERT INTO dbo.Envios (EnvioID, AyuntamientoID, ComercialID, NumeroReferencia, Estado, FechaEnvio, UsuarioModificadorID, Ubicacion) VALUES
(1, 1, 1, 'ENV-2024-001', 'Enviado', DATEADD(day, -10, GETDATE()), 1, 'Ayuntamiento de Madrid'),
(2, 2, 1, 'ENV-2024-002', 'Preparado', DATEADD(day, -5, GETDATE()), 1, 'Ayuntamiento de Barcelona'),
(3, 3, 2, 'ENV-2024-003', 'Pendiente', DATEADD(day, -2, GETDATE()), 2, 'Ayuntamiento de Valencia'),
(4, 4, 2, 'ENV-2024-004', 'Archivado', DATEADD(day, -1, GETDATE()), 2, 'Ayuntamiento de Sevilla');
SET IDENTITY_INSERT dbo.Envios OFF;
GO

-- =============================================
-- DATOS DE EJEMPLO: EnvíosDetalle
-- =============================================
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

-- =============================================
-- DATOS DE EJEMPLO: MovimientosStock
-- =============================================
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
PRINT '';
PRINT 'Resumen:';
PRINT '- ' + CAST((SELECT COUNT(*) FROM dbo.Comerciales) AS VARCHAR) + ' Comerciales';
PRINT '- ' + CAST((SELECT COUNT(*) FROM dbo.Ayuntamientos) AS VARCHAR) + ' Ayuntamientos';
PRINT '- ' + CAST((SELECT COUNT(*) FROM dbo.Categorias) AS VARCHAR) + ' Categorías';
PRINT '- ' + CAST((SELECT COUNT(*) FROM dbo.Productos) AS VARCHAR) + ' Productos';
PRINT '- ' + CAST((SELECT COUNT(*) FROM dbo.SIMs) AS VARCHAR) + ' SIMs';
PRINT '- ' + CAST((SELECT COUNT(*) FROM dbo.Usuarios) AS VARCHAR) + ' Usuarios';
PRINT '- ' + CAST((SELECT COUNT(*) FROM dbo.Envios) AS VARCHAR) + ' Envíos';
PRINT '- ' + CAST((SELECT COUNT(*) FROM dbo.EnviosDetalle) AS VARCHAR) + ' Detalles de Envío';
PRINT '- ' + CAST((SELECT COUNT(*) FROM dbo.MovimientosStock) AS VARCHAR) + ' Movimientos de Stock';
GO
