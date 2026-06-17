#!/usr/bin/env bash
# Post-deployment smoke test for FundiLink production.
#
# Checks:
#   1. The frontend responds with HTTP 200 over HTTPS.
#   2. The API liveness health endpoint responds with HTTP 200 over HTTPS.
#   3. The API DB health endpoint responds with HTTP 200 over HTTPS.
#
# Usage:
#   ./scripts/smoke-test.sh
#
# Override defaults via environment:
#   FRONTEND_URL=https://fundilink.zultek.co.za \
#   API_BASE_URL=https://api.fundilink.zultek.co.za \
#   ./scripts/smoke-test.sh

set -euo pipefail

FRONTEND_URL="${FRONTEND_URL:-https://fundilink.zultek.co.za}"
API_BASE_URL="${API_BASE_URL:-https://api.fundilink.zultek.co.za}"

check() {
  local name="$1"
  local url="$2"
  local status

  status=$(curl -s -o /dev/null -w "%{http_code}" "$url")

  if [ "$status" = "200" ]; then
    echo "OK   $name ($url) -> $status"
  else
    echo "FAIL $name ($url) -> $status"
    return 1
  fi
}

failed=0

check "Frontend"     "$FRONTEND_URL"         || failed=1
check "API health"   "$API_BASE_URL/health"  || failed=1
check "API DB health" "$API_BASE_URL/health/db" || failed=1

if [ "$failed" -ne 0 ]; then
  echo "Smoke test FAILED"
  exit 1
fi

echo "Smoke test PASSED"
