#!/bin/bash
set -e

echo "🔧 Ejecutando after_install.sh..."

# 👉 Exporta las variables para usar el SDK correcto
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$HOME/.dotnet:$PATH

# 👉 Navega al código fuente
cd /home/ubuntu/rdopartnumberssc

# 👉 Publica en la carpeta de artefactos
$DOTNET_ROOT/dotnet publish PartNumbers.csproj -c Release -o /home/ubuntu/partnumbers

echo "✅ Publicación completada en /home/ubuntu/partnumbers."