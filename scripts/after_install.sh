#!/bin/bash
set -e

echo "🔧 Ejecutando after_install.sh..."

# Navega al directorio del código
cd /home/ubuntu/partnumbers

# Publica la app (ajusta la ruta del proyecto si es necesario)
dotnet publish PartNumbers.csproj -c Release -o published

echo "✅ Publicación completada."