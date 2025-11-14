# ==================================================
# Script de Backup de Base de Datos StockLine
# ==================================================
# 
# Este script crea un backup de la base de datos StockLine
#
# USO:
#   .\backup-database.ps1 -Server "localhost" -User "sa" -Password "tu_password"
#

param(
    [Parameter(Mandatory=$false)]
    [string]$Server = "127.0.0.1,1433",
    
    [Parameter(Mandatory=$false)]
    [string]$User = "sa",
    
    [Parameter(Mandatory=$false)]
    [string]$Password = "",
    
    [Parameter(Mandatory=$false)]
    [string]$BackupPath = ""
)

Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "  Backup de Base de Datos StockLine" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si se proporcionó contraseña
if ([string]::IsNullOrEmpty($Password)) {
    $SecurePassword = Read-Host "Ingrese la contraseña de SQL Server" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecurePassword)
    $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

# Definir ruta de backup
if ([string]::IsNullOrEmpty($BackupPath)) {
    $BackupFolder = Join-Path $PSScriptRoot "Backups"
    if (-not (Test-Path $BackupFolder)) {
        New-Item -ItemType Directory -Path $BackupFolder | Out-Null
    }
    $Timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $BackupPath = Join-Path $BackupFolder "StockLine_Backup_$Timestamp.bak"
}

Write-Host "Configuración:" -ForegroundColor Yellow
Write-Host "  Servidor: $Server" -ForegroundColor White
Write-Host "  Usuario: $User" -ForegroundColor White
Write-Host "  Ruta de backup: $BackupPath" -ForegroundColor White
Write-Host ""

# Crear backup
Write-Host "Creando backup..." -ForegroundColor Yellow
$BackupQuery = "BACKUP DATABASE StockLine TO DISK='$BackupPath' WITH FORMAT, NAME='StockLine Full Backup';"

try {
    sqlcmd -S $Server -U $User -P $Password -Q $BackupQuery
    Write-Host "? Backup creado exitosamente" -ForegroundColor Green
    Write-Host ""
    Write-Host "Archivo de backup: $BackupPath" -ForegroundColor Cyan
    
    # Mostrar tamaño del archivo
    if (Test-Path $BackupPath) {
        $FileSize = (Get-Item $BackupPath).Length / 1MB
        Write-Host "Tamaño: $([math]::Round($FileSize, 2)) MB" -ForegroundColor White
    }
} catch {
    Write-Host "? Error al crear el backup" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "  ? Backup completado" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Cyan
