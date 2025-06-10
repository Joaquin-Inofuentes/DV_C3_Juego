# Guardá esto como FixGitMerge.ps1 y ponelo en la carpeta con los archivos

$folder = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host "Procesando archivos en $folder"

# Cambiá *.scena por *.* para todos los archivos
Get-ChildItem -Path $folder -Filter *.scena | ForEach-Object {

    $file = $_.FullName
    Write-Host "Procesando $file"

    $content = Get-Content $file -Raw

    # Regex para múltiples conflictos en un archivo (modo multilinea y singleline)
    # Captura la parte remota (segunda sección entre ======= y >>>>>>>)
    $pattern = '(?ms)<<<<<<< HEAD.*?=======\r?\n(.*?)\r?\n>>>>>>>.*?'

    $newContent = [regex]::Replace($content, $pattern, '$1')

    Set-Content -Path $file -Value $newContent
}

Write-Host "Terminado."
