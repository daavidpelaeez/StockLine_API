# StockLine API

API REST para la gestión de stock, envíos y administración de productos para ayuntamientos.

##  Características

- **Gestión de Productos**: Control completo del inventario con categorías
- **Control de Stock**: Seguimiento de movimientos de entrada y salida
- **Gestión de Envíos**: Administración de envíos a ayuntamientos
- **Gestión de SIMs**: Control de tarjetas SIM asociadas a productos
- **Usuarios y Roles**: Sistema de autenticación y autorización
- **Comerciales y Ayuntamientos**: Gestión de clientes y comerciales

##  Tecnologías

- **.NET 6.0**
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Core Web API**
- **Swagger/OpenAPI**

##  Requisitos Previos

- .NET 6.0 SDK o superior
- SQL Server 2019 o superior (o SQL Server Express)
- Visual Studio 2022 o Visual Studio Code

##  Configuración e Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/daavidpelaeez/StockLine_API.git
cd StockLine_API
```

### 2. Configurar la Base de Datos

#### Opción A: Instalación Automática (Recomendada)

```powershell
cd Database
.\install-database.ps1 -Server "localhost" -User "sa" -Password "tu_password" -IncludeSampleData
```

#### Opción B: Instalación Manual

```bash
# Crear el schema
sqlcmd -S localhost -U sa -P tu_password -i Database/StockLine_Database_Schema.sql

# (Opcional) Insertar datos de ejemplo
sqlcmd -S localhost -U sa -P tu_password -i Database/StockLine_Sample_Data.sql
```

Ver más detalles en [Database/README.md](Database/README.md)

### 3. Configurar la Cadena de Conexión

Actualiza `appsettings.json` con tu configuración:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=StockLine;User Id=sa;Password=tu_password;TrustServerCertificate=True;"
  }
}
```

### 4. Restaurar Paquetes y Ejecutar

```bash
dotnet restore
dotnet run
```

La API estará disponible en: `https://localhost:7XXX` (el puerto se muestra en la consola)

##  Documentación de la API

Una vez que la aplicación esté en ejecución, accede a la documentación Swagger en:

```
https://localhost:7XXX/swagger
```

##  Estructura del Proyecto

```
StockLine_API/
 Controllers/          # Controladores de la API
 Services/            # Lógica de negocio
 Models/              # Modelos de entidades
 DTOs/                # Data Transfer Objects
 Database/            # Scripts de base de datos
 StockLine_Database_Schema.sql
 StockLine_Sample_Data.sql
 install-database.ps1
 README.md
 StockLineContext.cs  # Contexto de Entity Framework
 Program.cs           # Punto de entrada
 appsettings.json     # Configuración
```

##  Credenciales por Defecto

**Usuario Administrador:**
- Email: `admin@stockline.com`
- Password: `password123`

 **IMPORTANTE**: Cambia estas credenciales en producción.

##  Endpoints Principales

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrar usuario

### Productos
- `GET /api/productos` - Listar productos
- `POST /api/productos` - Crear producto
- `PUT /api/productos/{id}` - Actualizar producto
- `DELETE /api/productos/{id}` - Eliminar producto

### Envíos
- `GET /api/envios` - Listar envíos
- `POST /api/envios` - Crear envío
- `PATCH /api/envios/{id}/estado` - Actualizar estado

### Ayuntamientos
- `GET /api/ayuntamientos` - Listar ayuntamientos
- `POST /api/ayuntamientos` - Crear ayuntamiento
- `PUT /api/ayuntamientos/{id}` - Actualizar ayuntamiento
- `DELETE /api/ayuntamientos/{id}` - Eliminar ayuntamiento

Ver documentación completa en Swagger.

##  Base de Datos

El proyecto incluye scripts SQL completos en la carpeta `Database/`:

- **Schema completo** con todas las tablas y relaciones
- **Datos iniciales** (roles, usuario administrador)
- **Datos de ejemplo** (opcional, para desarrollo/testing)
- **Script de instalación automatizado** (PowerShell)

##  Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

##  Licencia

Este proyecto es privado y propiedad de StockLine.

##  Autores

- David Peláez - [@daavidpelaeez](https://github.com/daavidpelaeez)

##  Soporte

Para preguntas o problemas, abre un issue en el repositorio.
