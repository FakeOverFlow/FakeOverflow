#!/bin/bash
set -e

COMPOSE_DIR="/opt/fakeoverflow"
COMPOSE_FILE="$COMPOSE_DIR/docker-compose.yml"
LOG_FILE="$COMPOSE_DIR/deploy.log"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log() {
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

echo -e "${BLUE}=== FakeOverflow Deployment Script ===${NC}"
log "Starting deployment process"

if [ ! -f "$COMPOSE_FILE" ]; then
    echo -e "${RED}Error: docker-compose.yml not found at $COMPOSE_FILE${NC}"
    log "ERROR: docker-compose.yml not found"
    exit 1
fi

cd "$COMPOSE_DIR"
log "Changed to directory: $COMPOSE_DIR"

echo -e "${YELLOW}Step 1: Stopping all services...${NC}"
log "Stopping docker compose services"
docker compose down || {
    echo -e "${RED}Warning: Failed to stop some services${NC}"
    log "WARNING: Failed to stop some services"
}

echo -e "${YELLOW}Step 2: Pruning unused images...${NC}"
log "Pruning unused docker images"
docker image prune -f
log "Docker image prune completed"

echo -e "${YELLOW}Step 3: Pulling latest images from docker compose...${NC}"
log "Pulling latest images"
docker compose pull || {
    echo -e "${RED}Error: Failed to pull images${NC}"
    log "ERROR: Failed to pull images"
    exit 1
}
log "Successfully pulled latest images"

echo -e "${YELLOW}Step 4: Starting services in detached mode...${NC}"
log "Starting services with docker compose up -d"
docker compose up -d || {
    echo -e "${RED}Error: Failed to start services${NC}"
    log "ERROR: Failed to start services"
    exit 1
}

echo -e "${YELLOW}Step 5: Checking service status...${NC}"
sleep 5
docker compose ps

echo -e "${GREEN}=== Deployment completed successfully! ===${NC}"
log "Deployment completed successfully"

echo -e "${BLUE}Service logs (last 10 lines):${NC}"
docker compose logs --tail=10

echo ""
echo -e "${BLUE}To view full logs: docker compose logs -f${NC}"
echo -e "${BLUE}To check status: docker compose ps${NC}"
echo -e "${BLUE}Deployment log: $LOG_FILE${NC}"