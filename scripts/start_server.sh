#!/bin/bash
set -e

echo "🚀 Ejecutando start_server.sh..."

# Reinicia el servicio de tu API
sudo systemctl restart rdopartnumbers.service

echo "✅ Servicio reiniciado correctamente."