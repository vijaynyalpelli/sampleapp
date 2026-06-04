#!/bin/bash
# Quick Docker Commands Reference for MySampleApp

# ========================================
# BUILD & RUN WITH DOCKER COMPOSE
# ========================================

# Build the Docker image
docker-compose build

# Run the container in background
docker-compose up -d

# View real-time logs
docker-compose logs -f

# Stop the container
docker-compose down

# ========================================
# BUILD & RUN WITH DOCKER DIRECTLY
# ========================================

# Build the image
docker build -t mysampleapp:latest .

# Run the container
docker run -d -p 5000:5000 -p 5001:5001 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  --name mysampleapp \
  mysampleapp:latest

# ========================================
# TESTING ENDPOINTS
# ========================================

# Test root endpoint
curl http://localhost:5000/

# Test health endpoint
curl http://localhost:5000/health

# Test weather forecast endpoint
curl http://localhost:5000/weatherforecast

# ========================================
# CONTAINER MANAGEMENT
# ========================================

# List running containers
docker ps

# View container logs
docker logs mysampleapp

# View container logs (follow/tail)
docker logs -f mysampleapp

# Execute command in running container
docker exec -it mysampleapp bash

# Stop container
docker stop mysampleapp

# Start stopped container
docker start mysampleapp

# Remove container
docker rm mysampleapp

# ========================================
# IMAGE MANAGEMENT
# ========================================

# List all images
docker images

# Remove image
docker rmi mysampleapp:latest

# Tag image for registry
docker tag mysampleapp:latest myregistry/mysampleapp:1.0.0

# Push to registry
docker push myregistry/mysampleapp:1.0.0

# ========================================
# DEBUGGING
# ========================================

# Check container health status
docker inspect --format='{{json .State.Health}}' mysampleapp | python -m json.tool

# View all container info
docker inspect mysampleapp

# Resource usage statistics
docker stats mysampleapp

# View running processes inside container
docker top mysampleapp

# ========================================
# CLEANUP
# ========================================

# Remove all stopped containers
docker container prune

# Remove unused images
docker image prune

# Remove all unused volumes
docker volume prune

# Remove everything (containers, images, volumes, networks)
docker system prune -a

# ========================================
# COMPOSE COMMANDS
# ========================================

# Start services
docker-compose up

# Start services in background
docker-compose up -d

# Stop services
docker-compose stop

# Remove services
docker-compose down

# View service logs
docker-compose logs

# Restart services
docker-compose restart

# Scale service (run multiple instances)
docker-compose up -d --scale sampleapp=3
