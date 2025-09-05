#!/bin/bash

set -e

REPLICA_COUNT=${1:-2}
IMAGE_NAME=${2:-"mirze1993/tempa:latest"}
COMPOSE_FILE="docker-compose.generated.yml"
NGINX_CONF="nginx.generated.conf"

echo "➡️  $REPLICA_COUNT ədəd replica üçün $COMPOSE_FILE yaradılır..."

# 1. Nginx conf generasiya olunur
cat > $NGINX_CONF <<EOF
events {}

http {
  upstream dotnet_app {
EOF

for i in $(seq 1 $REPLICA_COUNT); do
  echo "    server c_auth$i:5197;" >> $NGINX_CONF
done

cat >> $NGINX_CONF <<EOF
  }

  server {
    listen 80;
    location / {
      proxy_pass http://dotnet_app;
    }
  }
}
EOF

# 2. Compose YAML generasiya olunur
cat > $COMPOSE_FILE <<EOF
version: '3.8'

services:
EOF

for i in $(seq 1 $REPLICA_COUNT); do
cat >> $COMPOSE_FILE <<EOF
  c_auth$i:
    image: $IMAGE_NAME
    container_name: c_auth$i
    restart: unless-stopped
    networks:
      - net_backend
      - shared-web
    expose:
      - "5197"  
    healthcheck:
      test: curl -sS http://localhost:5197/swagger/index.html || exit 1
      interval: 1m30s
      timeout: 10s
      retries: 3
      start_period: 10s

EOF
done

# 3. Nginx servisi əlavə olunur
cat >> $COMPOSE_FILE <<EOF
  c_auth_nginx:
    image: nginx:alpine
    container_name: nginx
    volumes:
      - ./nginx.generated.conf:/etc/nginx/nginx.conf:ro
    ports:
      - "5193:80"
    restart: unless-stopped
    networks:
      - shared-web

networks:
  net_backend:
    driver: bridge
  shared-web:
    external: true
EOF



echo "✅ Deploy və nginx konfiqurasiyası tamamlandı!"