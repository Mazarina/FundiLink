#!/usr/bin/env bash
# Creates a timestamped pg_dump backup of the FundiLink production database.
#
# Usage:
#   ./scripts/backup-db.sh
#
# Expects to be run on the VPS from /opt/fundilink/app, with the production
# env file at /opt/fundilink/env/.env and the fundilink-db container running.
# Writes backups to /opt/fundilink/backups.

set -euo pipefail

APP_DIR="${APP_DIR:-/opt/fundilink/app}"
ENV_FILE="${ENV_FILE:-/opt/fundilink/env/.env}"
BACKUP_DIR="${BACKUP_DIR:-/opt/fundilink/backups}"
COMPOSE_FILE="${COMPOSE_FILE:-$APP_DIR/docker-compose.production.yml}"

# shellcheck disable=SC1090
source "$ENV_FILE"

mkdir -p "$BACKUP_DIR"

TIMESTAMP=$(date -u +%Y%m%dT%H%M%SZ)
BACKUP_FILE="$BACKUP_DIR/fundilink-db-${TIMESTAMP}.sql.gz"

echo "Backing up database '${POSTGRES_DB}' to ${BACKUP_FILE}..."

docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" exec -T fundilink-db \
  pg_dump -U "$POSTGRES_USER" "$POSTGRES_DB" | gzip > "$BACKUP_FILE"

echo "Backup complete: ${BACKUP_FILE}"
