#!/bin/bash
set -e

echo "🚀 Ejecutando start_server.sh..."

# Restart the api service
sudo systemctl restart PartNumbersService

echo "✅ Servicio reiniciado correctamente."