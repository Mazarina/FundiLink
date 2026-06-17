#!/usr/bin/env bash
# Creates a timestamped pg_dump backup of the FundiLink production database.
#
# Usage (run on the VPS):
#   /opt/fundilink/app/scripts/backup-db.sh
#
# Reads credentials from /opt/fundilink/env/fundilink.env.
# Writes backups to /opt/fundilink/backups.
# Keeps the 7 most recent daily backups and removes older ones.

set -euo pipefail

APP_DIR="${APP_DIR:-/opt/fundilink/app}"
ENV_FILE="${ENV_FILE:-/opt/fundilink/env/fundilink.env}"
BACKUP_DIR="${BACKUP_DIR:-/opt/fundilink/backups}"
COMPOSE_FILE="${COMPOSE_FILE:-$APP_DIR/docker-compose.production.yml}"
KEEP_DAYS="${KEEP_DAYS:-7}"

# shellcheck disable=SC1090
source "$ENV_FILE"

mkdir -p "$BACKUP_DIR"

TIMESTAMP=$(date -u +%Y%m%dT%H%M%SZ)
BACKUP_FILE="$BACKUP_DIR/fundilink-db-${TIMESTAMP}.sql.gz"

echo "Backing up database '${POSTGRES_DB}' to ${BACKUP_FILE}..."

docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" --project-name fundilink \
  exec -T fundilink-db \
  pg_dump -U "$POSTGRES_USER" "$POSTGRES_DB" | gzip > "$BACKUP_FILE"

echo "Backup complete: ${BACKUP_FILE}"

# Retain only the most recent $KEEP_DAYS backups.
find "$BACKUP_DIR" -maxdepth 1 -name 'fundilink-db-*.sql.gz' \
  | sort -r \
  | tail -n +$((KEEP_DAYS + 1)) \
  | xargs -r rm --
echo "Retention cleanup done (kept last ${KEEP_DAYS} backups)."
