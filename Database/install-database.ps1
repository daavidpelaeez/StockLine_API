# ==================================================
# Script de Instalación de Base de Datos StockLine
# ==================================================
# 
# Este script facilita la creación e inicialización
# de la base de datos StockLine
#
# USO:
#   .\install-database.ps1 -Server "localhost" -User "sa" -Password "tu_password"
#

param(
    [Parameter(Mandatory=$false)]
    [string]$Server = "127.0.0.1,1433",
    
    [Parameter(Mandatory=$false)]
    [string]$User = "sa",
    
    [Parameter(Mandatory=$false)]
    [string]$Password = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$IncludeSampleData = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$DropExisting = $false
)

Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "  Instalación de Base de Datos StockLine" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si se proporcionó contraseña
if ([string]::IsNullOrEmpty($Password)) {
    $SecurePassword = Read-Host "Ingrese la contraseña de SQL Server" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecurePassword)
    $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

$ScriptPath = $PSScriptRoot
$SchemaScript = Join-Path $ScriptPath "StockLine_Database_Schema.sql"
$SampleDataScript = Join-Path $ScriptPath "StockLine_Sample_Data.sql"

# Verificar que existan los archivos
if (-not (Test-Path $SchemaScript)) {
    Write-Host "ERROR: No se encontró el archivo de schema: $SchemaScript" -ForegroundColor Red
    exit 1
}

Write-Host "Configuración:" -ForegroundColor Yellow
Write-Host "  Servidor: $Server" -ForegroundColor White
Write-Host "  Usuario: $User" -ForegroundColor White
Write-Host "  Incluir datos de ejemplo: $IncludeSampleData" -ForegroundColor White
Write-Host ""

# Eliminar base de datos existente si se especificó
if ($DropExisting) {
    Write-Host "Eliminando base de datos existente..." -ForegroundColor Yellow
    $DropDbQuery = "IF EXISTS (SELECT name FROM sys.databases WHERE name = N'StockLine') BEGIN ALTER DATABASE StockLine SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE StockLine; END"
    try {
        sqlcmd -S $Server -U $User -P $Password -Q $DropDbQuery
        Write-Host "? Base de datos eliminada" -ForegroundColor Green
    } catch {
        Write-Host "? No se pudo eliminar la base de datos (puede que no exista)" -ForegroundColor Yellow
    }
    Write-Host ""
}

# Ejecutar script de schema
Write-Host "Ejecutando script de schema..." -ForegroundColor Yellow
try {
    sqlcmd -S $Server -U $User -P $Password -i $SchemaScript
    Write-Host "? Schema creado exitosamente" -ForegroundColor Green
} catch {
    Write-Host "? Error al crear el schema" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
Write-Host ""

# Ejecutar script de datos de ejemplo si se especificó
if ($IncludeSampleData) {
    if (Test-Path $SampleDataScript) {
        Write-Host "Insertando datos de ejemplo..." -ForegroundColor Yellow
        try {
            sqlcmd -S $Server -U $User -P $Password -i $SampleDataScript
            Write-Host "? Datos de ejemplo insertados" -ForegroundColor Green
        } catch {
            Write-Host "? Error al insertar datos de ejemplo" -ForegroundColor Yellow
            Write-Host $_.Exception.Message -ForegroundColor Yellow
        }
    } else {
        Write-Host "? No se encontró el archivo de datos de ejemplo" -ForegroundColor Yellow
    }
    Write-Host ""
}

Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "  ? Instalación completada" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Siguiente paso: Actualiza tu appsettings.json con:" -ForegroundColor Yellow
Write-Host ""
Write-Host '  "ConnectionStrings": {' -ForegroundColor White
Write-Host "    `"DefaultConnection`": `"Server=$Server;Database=StockLine;User Id=$User;Password=***;TrustServerCertificate=True;`"" -ForegroundColor White
Write-Host '  }' -ForegroundColor White
Write-Host ""
