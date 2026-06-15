# DEPLOYMENT.md — FundiLink by ZulTek

This guide describes how to deploy FundiLink to a single Ubuntu/Debian VPS using
Docker Compose, a host-level Nginx reverse proxy, and Let's Encrypt (Certbot) for
TLS, on the `zultek.co.za` domain.

- Frontend: `https://fundilink.zultek.co.za`
- API: `https://api-fundilink.zultek.co.za`

It assumes wildcard DNS for `*.zultek.co.za` (or explicit `A`/`AAAA` records for the
two subdomains above) already points at the VPS's public IP.

---

## 1. Architecture overview

```
Internet
  │
  ▼
Host Nginx (ports 80/443, Certbot-managed TLS)
  ├─ fundilink.zultek.co.za      → 127.0.0.1:5173  (fundilink-frontend container)
  └─ api-fundilink.zultek.co.za  → 127.0.0.1:5000   (fundilink-api container)

Docker Compose (docker-compose.production.yml)
  ├─ fundilink-frontend  (Nginx + static React build, port 8080 → host 127.0.0.1:5173)
  ├─ fundilink-api       (.NET 8 API, port 8080 → host 127.0.0.1:5000)
  └─ fundilink-db        (PostgreSQL 16, internal network only, not published)
```

- `fundilink-db` is on an `internal: true` Docker network and is never reachable
  from the host or the internet.
- `fundilink-api` and `fundilink-frontend` are published only on `127.0.0.1`,
  so the host Nginx is the only public entry point.
- Uploaded documents are stored on the host at `/opt/fundilink/uploads`, bind-mounted
  into the API container at `/app/uploads`. This volume is **not** served by Nginx
  and has no static file route — access is only through authenticated API endpoints.

---

## 2. Persistent VPS folder layout

Create the following structure under `/opt/fundilink` (done in section 4):

| Path | Purpose |
|---|---|
| `/opt/fundilink/app` | Application source/checkout (deployed via CI) |
| `/opt/fundilink/env` | `.env` file with production secrets (never committed) |
| `/opt/fundilink/uploads` | Learner-uploaded documents (bind-mounted into the API container) |
| `/opt/fundilink/backups` | Database backups produced by `scripts/backup-db.sh` |
| `/opt/fundilink/nginx` | Optional copies of host Nginx configs for reference |
| `/opt/fundilink/logs` | Reserved for log output if/when container logs are redirected here |

---

## 3. Prerequisites on the VPS

- Ubuntu 22.04/24.04 (or Debian 12) with a non-root sudo-capable user
- A DNS `A` record (or wildcard) for `fundilink.zultek.co.za` and
  `api-fundilink.zultek.co.za` pointing at the VPS's public IP
- Outbound internet access (for pulling base images and Certbot)

---

## 4. First-time VPS setup

Run these commands **once**, as a sudo-capable user (replace `deploy` with your
actual deploy username if different).

### 4.1 Create a dedicated deploy user (if one doesn't exist)

```bash
sudo adduser deploy
sudo usermod -aG sudo deploy
# Switch to the deploy user for the rest of this guide
su - deploy
```

### 4.2 Install Docker and Docker Compose

```bash
sudo apt-get update
sudo apt-get install -y ca-certificates curl gnupg

sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo \"$VERSION_CODENAME\") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin

# Allow the deploy user to run docker without sudo
sudo usermod -aG docker $USER
newgrp docker
```

### 4.3 Install Nginx and Certbot

```bash
sudo apt-get install -y nginx certbot python3-certbot-nginx
```

### 4.4 Create persistent application folders

```bash
sudo mkdir -p /opt/fundilink/{app,env,uploads,backups,nginx,logs}
sudo chown -R $USER:$USER /opt/fundilink
chmod 700 /opt/fundilink/env
```

### 4.5 Configure the firewall (ufw)

```bash
sudo ufw allow OpenSSH
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
sudo ufw status
```

> Note: PostgreSQL (5432) and the API/frontend container ports (5000/5173) are
> **not** opened — they are bound to `127.0.0.1` only and accessed exclusively via
> the host Nginx reverse proxy.

### 4.6 Generate an SSH deploy key for GitHub Actions

On your local machine or the VPS, generate a dedicated key pair for CI:

```bash
ssh-keygen -t ed25519 -C "fundilink-deploy" -f fundilink_deploy_key -N ""
```

Append the **public** key to the deploy user's `~/.ssh/authorized_keys` on the VPS:

```bash
cat fundilink_deploy_key.pub >> ~/.ssh/authorized_keys
```

The **private** key (`fundilink_deploy_key`, full contents including
`-----BEGIN OPENSSH PRIVATE KEY-----` / `-----END...-----` lines) becomes the
`VPS_SSH_KEY` GitHub Secret (see section 6).

---

## 5. Initial code checkout

The GitHub Actions deploy workflow `rsync`s the repository into
`/opt/fundilink/app` on every deploy, so an initial checkout isn't strictly
required — but for a first manual deployment (before CI is configured), clone the
repo once:

```bash
git clone <your-repo-url> /opt/fundilink/app
cd /opt/fundilink/app
git checkout main
```

---

## 6. GitHub Secrets configuration

In the GitHub repository, go to **Settings → Environments → production** (or
**Settings → Secrets and variables → Actions** for repository-level secrets) and
add the following secrets. These are consumed by `.github/workflows/deploy.yml`.

| Secret | Example / notes |
|---|---|
| `VPS_HOST` | VPS public IP or hostname, e.g. `203.0.113.10` |
| `VPS_USER` | `deploy` |
| `VPS_SSH_KEY` | Private key contents from section 4.6 |
| `VPS_APP_DIR` | `/opt/fundilink/app` |
| `POSTGRES_DB` | `fundilink_prod` |
| `POSTGRES_USER` | `fundilink_prod` |
| `POSTGRES_PASSWORD` | Strong random password — generate with `openssl rand -base64 32` |
| `JWT_SECRET` | Strong random secret, ≥ 32 chars — generate with `openssl rand -base64 48` |
| `JWT_ISSUER` | `FundiLink` |
| `JWT_AUDIENCE` | `FundiLink` |
| `CORS_ALLOWED_ORIGINS` | `https://fundilink.zultek.co.za` |
| `FRONTEND_URL` | `https://fundilink.zultek.co.za` |
| `API_BASE_URL` | `https://api-fundilink.zultek.co.za` |
| `VITE_API_BASE_URL` | `https://api-fundilink.zultek.co.za` |
| `UPLOADS_PATH` | `/opt/fundilink/uploads` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

> `DATABASE_URL`/connection string, JWT expiry, document size limits, and rate
> limit settings are derived inside `docker-compose.production.yml` from the
> values above (see `.env.production.example` for the full list of optional
> overrides such as `JWT_EXPIRY_MINUTES`, `DOCUMENT_MAX_SIZE_BYTES`,
> `RATE_LIMIT_PERMIT_LIMIT`, `RATE_LIMIT_WINDOW_SECONDS`). If you want to override
> these, add them to `/opt/fundilink/env/.env` manually after the first deploy, or
> extend the "Write production environment file" step in `deploy.yml`.

**Never** commit real values for any of the above. `.env.production.example` at
the repo root documents every variable with placeholder values only.

---

## 7. First deployment

### 7.1 Manual first deploy (recommended before enabling CI/CD)

```bash
cd /opt/fundilink/app

# Create the env file from the template and fill in real values
cp .env.production.example /opt/fundilink/env/.env
nano /opt/fundilink/env/.env   # fill in POSTGRES_PASSWORD, JWT_SECRET, etc.
chmod 600 /opt/fundilink/env/.env

# Build and start all services
docker compose -f docker-compose.production.yml --env-file /opt/fundilink/env/.env build
docker compose -f docker-compose.production.yml --env-file /opt/fundilink/env/.env up -d

# Check container status
docker compose -f docker-compose.production.yml ps
```

EF Core migrations run automatically on API startup (see `Program.cs`) — no
separate migration command is needed.

### 7.2 Enable GitHub Actions CI/CD

Once the manual first deploy succeeds and the GitHub Secrets from section 6 are
configured, push to `main` (or run the **Deploy to Production** workflow manually
via `workflow_dispatch`). The workflow will:

1. `rsync` the repository to `/opt/fundilink/app`
2. Write `/opt/fundilink/env/.env` from the configured secrets
3. `docker compose build` and `docker compose up -d`
4. Poll `${API_BASE_URL}/health` until it returns 200 (10 retries, 5s apart)

---

## 8. Nginx + Certbot setup

The repo includes ready-to-use host Nginx configs at `nginx/fundilink-frontend.conf`
and `nginx/fundilink-api.conf`.

```bash
# Copy configs into place
sudo cp /opt/fundilink/app/nginx/fundilink-frontend.conf /etc/nginx/sites-available/
sudo cp /opt/fundilink/app/nginx/fundilink-api.conf /etc/nginx/sites-available/

sudo ln -s /etc/nginx/sites-available/fundilink-frontend.conf /etc/nginx/sites-enabled/
sudo ln -s /etc/nginx/sites-available/fundilink-api.conf /etc/nginx/sites-enabled/

# Validate and reload
sudo nginx -t
sudo systemctl reload nginx

# Issue certificates (Certbot rewrites the server blocks to add SSL + HTTP->HTTPS redirect)
sudo certbot --nginx -d fundilink.zultek.co.za
sudo certbot --nginx -d api-fundilink.zultek.co.za

# Confirm auto-renewal is scheduled
sudo systemctl status certbot.timer
```

---

## 9. Verification

After deployment and Nginx/Certbot setup, verify the deployment:

```bash
# Container health
docker compose -f /opt/fundilink/app/docker-compose.production.yml ps

# API liveness and DB readiness (from the VPS or anywhere)
curl -i https://api-fundilink.zultek.co.za/health
curl -i https://api-fundilink.zultek.co.za/health/db

# Frontend loads
curl -i https://fundilink.zultek.co.za

# Or run the bundled smoke test
FRONTEND_URL=https://fundilink.zultek.co.za \
API_BASE_URL=https://api-fundilink.zultek.co.za \
/opt/fundilink/app/scripts/smoke-test.sh
```

Expect `200 OK` from all three checks.

---

## 10. Rollback

To roll back to a previous version:

```bash
cd /opt/fundilink/app
git fetch origin
git checkout <previous-good-commit-or-tag>

docker compose -f docker-compose.production.yml --env-file /opt/fundilink/env/.env build
docker compose -f docker-compose.production.yml --env-file /opt/fundilink/env/.env up -d
```

EF Core migrations only ever apply forward (no automatic down-migrations). If a
rollback requires reverting a database migration, do so manually and deliberately
— **never** run destructive database commands without a fresh backup (see
section 11) and explicit sign-off.

---

## 11. Backups

`scripts/backup-db.sh` creates a timestamped, gzip-compressed `pg_dump` of the
production database in `/opt/fundilink/backups`:

```bash
/opt/fundilink/app/scripts/backup-db.sh
```

Recommended: schedule this via cron for a daily backup, e.g.:

```bash
crontab -e
# Add:
0 3 * * * /opt/fundilink/app/scripts/backup-db.sh >> /opt/fundilink/logs/backup.log 2>&1
```

### Restore from a backup

```bash
cd /opt/fundilink/app
source /opt/fundilink/env/.env

gunzip -c /opt/fundilink/backups/fundilink-db-<timestamp>.sql.gz | \
  docker compose -f docker-compose.production.yml --env-file /opt/fundilink/env/.env \
  exec -T fundilink-db psql -U "$POSTGRES_USER" "$POSTGRES_DB"
```

> Restoring overwrites existing data. Only do this deliberately, with a fresh
> backup of the current state taken first, and with explicit confirmation from
> the team.

---

## 12. Troubleshooting

| Symptom | Check |
|---|---|
| `502 Bad Gateway` from Nginx | Is the container running? `docker compose ps`. Check `docker compose logs fundilink-api` / `fundilink-frontend`. |
| API returns 500 on every request | Check `docker compose logs fundilink-api` for startup errors (e.g. missing `JwtSettings__SecretKey`, DB connection failure). |
| `/health/db` returns unhealthy | Check `docker compose logs fundilink-db` and confirm `POSTGRES_*` env vars match between `fundilink-db` and `fundilink-api`. |
| Migrations not applied | Confirm `ASPNETCORE_ENVIRONMENT=Production` (or any relational provider) — migrations are skipped only for the InMemory provider used in tests. Check API startup logs for `Applying migration...` entries. |
| Uploads failing with 413 | Increase `client_max_body_size` in `nginx/fundilink-api.conf` (currently 12MB) if `DOCUMENT_MAX_SIZE_BYTES` is increased beyond 10MB. |
| CORS errors in browser | Confirm `CORS_ALLOWED_ORIGINS` exactly matches `https://fundilink.zultek.co.za` (no trailing slash). |
| Certbot renewal failing | `sudo certbot renew --dry-run`; ensure port 80 is reachable (ufw, DNS). |
| Need to inspect uploaded documents on disk | `/opt/fundilink/uploads` on the host — files are named by internal storage keys, not original filenames, and are never served directly by Nginx. |

---

## 13. Local development (unaffected)

This production setup does not change local development. Continue using:

```bash
docker compose up -d        # PostgreSQL via docker/docker-compose.yml (if present)
dotnet run --project src/FundiLink.Api
cd src/fundilink-web && npm run dev
```

The frontend's `.env` (or absence of `VITE_API_BASE_URL`) continues to use the
Vite dev proxy to the local API, as before.
