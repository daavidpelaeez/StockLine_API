# Base de Datos StockLine

Este directorio contiene los scripts de base de datos para el proyecto StockLine API.

## Archivos

- **StockLine_Database_Schema.sql**: Script completo de creación de la base de datos, incluyendo todas las tablas, relaciones, índices y datos iniciales.
- **StockLine_Sample_Data.sql**: Script con datos de ejemplo para desarrollo y testing.
- **install-database.ps1**: Script PowerShell para instalación automatizada de la base de datos.
- **backup-database.ps1**: Script PowerShell para crear backups de la base de datos.

## Estructura de la Base de Datos

### Tablas Principales

1. **Roles**: Roles de usuario del sistema
2. **Usuarios**: Usuarios del sistema con autenticación
3. **Comerciales**: Información de los comerciales
4. **Ayuntamientos**: Información de los ayuntamientos
5. **Categorias**: Categorías de productos
6. **Productos**: Catálogo de productos
7. **SIMs**: Tarjetas SIM disponibles
8. **Envios**: Registro de envíos
9. **EnviosDetalle**: Detalles de los productos en cada envío
10. **MovimientosStock**: Historial de movimientos de stock

### Relaciones

```
Roles (1) ??< (N) Usuarios
Comerciales (1) ??< (N) Ayuntamientos
Comerciales (1) ??< (N) Envios
Categorias (1) ??< (N) Productos
Productos (1) ??< (N) SIMs
Productos (1) ??< (N) MovimientosStock
Ayuntamientos (1) ??< (N) Envios
Envios (1) ??< (N) EnviosDetalle
Usuarios (1) ??< (N) MovimientosStock
Usuarios (1) ??< (N) Envios (como UsuarioModificador)
```

## Cómo Usar

### Instalación Completa (Recomendada)

Usar el script de PowerShell para instalación automatizada:

```powershell
# Con datos de ejemplo
.\install-database.ps1 -Server "localhost" -User "sa" -Password "tu_password" -IncludeSampleData

# Sin datos de ejemplo
.\install-database.ps1 -Server "localhost" -User "sa" -Password "tu_password"

# Eliminar base de datos existente y recrear
.\install-database.ps1 -Server "localhost" -User "sa" -Password "tu_password" -DropExisting -IncludeSampleData
```

### Instalación Manual

Para crear la base de datos manualmente:

```bash
# 1. Crear el schema
sqlcmd -S localhost -U sa -P tu_password -i StockLine_Database_Schema.sql

# 2. (Opcional) Insertar datos de ejemplo
sqlcmd -S localhost -U sa -P tu_password -i StockLine_Sample_Data.sql
```

### Configuración de Conexión

Asegúrate de actualizar la cadena de conexión en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tu_servidor;Database=StockLine;User Id=tu_usuario;Password=tu_contraseña;TrustServerCertificate=True;"
  }
}
```

## Datos Iniciales

El script incluye los siguientes datos iniciales:

### Roles
- **Administrador**: Acceso completo al sistema
- **Usuario**: Acceso limitado al sistema
- **Comercial**: Acceso para comerciales

### Usuario por Defecto
- **Email**: admin@stockline.com
- **Password**: password123
- **Rol**: Administrador

?? **IMPORTANTE**: Cambia la contraseña del usuario administrador en producción.

## Datos de Ejemplo (Opcional)

Si ejecutas el script `StockLine_Sample_Data.sql`, se insertarán:

- 3 Comerciales
- 5 Ayuntamientos
- 6 Categorías de productos
- 8 Productos
- 6 SIMs
- 4 Usuarios
- 3 Envíos con detalles
- 5 Movimientos de stock

## Backup y Restore

### Crear Backup (Automatizado)

```powershell
# Crear backup con timestamp
.\backup-database.ps1 -Server "localhost" -User "sa" -Password "tu_password"

# Especificar ruta de backup
.\backup-database.ps1 -Server "localhost" -User "sa" -Password "tu_password" -BackupPath "C:\Backups\MiBackup.bak"
```

Los backups automáticos se guardan en `Database/Backups/` con timestamp.

### Crear Backup (Manual)

```bash
sqlcmd -S servidor -U usuario -P contraseña -Q "BACKUP DATABASE StockLine TO DISK='C:\Backup\StockLine.bak' WITH FORMAT"
```

### Restaurar Backup

```bash
sqlcmd -S servidor -U usuario -P contraseña -Q "RESTORE DATABASE StockLine FROM DISK='C:\Backup\StockLine.bak' WITH REPLACE"
```

## Actualizaciones del Schema

Cuando se realicen cambios en el schema de la base de datos:

1. Actualiza el archivo `StockLine_Database_Schema.sql`
2. Documenta los cambios en este README
3. Considera crear un script de migración específico si es necesario
4. Prueba la instalación desde cero con el script actualizado

## Notas Técnicas

- La base de datos está diseñada para **SQL Server 2019+**
- Se utiliza **Entity Framework Core** como ORM
- Los campos de fecha usan `DATETIME` con valores por defecto `GETDATE()`
- Se han creado **índices** en las columnas más consultadas para mejorar el rendimiento
- Las relaciones de clave foránea están configuradas con `ON DELETE RESTRICT` para prevenir eliminaciones accidentales
- Los estados de envíos están restringidos mediante `CHECK` constraints: 'Pendiente', 'Preparado', 'Enviado'
- Los tipos de movimiento de stock están restringidos: 'Entrada', 'Salida'

## Solución de Problemas

### Error: "Login failed for user 'sa'"
- Verifica que SQL Server esté ejecutándose
- Verifica que la contraseña sea correcta
- Asegúrate de que la autenticación SQL Server esté habilitada

### Error: "Database already exists"
- Usa el parámetro `-DropExisting` en el script de instalación
- O elimina manualmente la base de datos antes de ejecutar el script

### Error al conectar desde la aplicación
- Verifica la cadena de conexión en `appsettings.json`
- Asegúrate de que SQL Server permita conexiones remotas si aplica
- Verifica que el firewall permita conexiones al puerto 1433

## Seguridad

?? **Importantes consideraciones de seguridad:**

1. **NUNCA** subas `appsettings.json` con credenciales reales al repositorio
2. Usa `appsettings.example.json` como plantilla
3. Cambia las contraseñas por defecto en producción
4. Considera usar **Azure Key Vault** o **User Secrets** para credenciales sensibles
5. Implementa políticas de contraseñas robustas para usuarios
6. Revisa los permisos de la base de datos en producción

## Mantenimiento

### Reindexación (Recomendada mensualmente)

```sql
USE StockLine;
GO
EXEC sp_MSforeachtable @command1="DBCC DBREINDEX ('?')";
GO
```

### Actualizar Estadísticas

```sql
USE StockLine;
GO
EXEC sp_updatestats;
GO
```

### Verificar Integridad

```sql
USE StockLine;
GO
DBCC CHECKDB('StockLine');
GO
