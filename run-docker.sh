#!/bin/bash
set -e  # Exit on error

echo "Stopping any running containers..."
docker-compose down

echo "Cleaning up volumes..."
docker-compose down -v

echo "Building and starting containers..."
docker-compose up --build -d

echo "Waiting for services to start..."
sleep 45

echo "Checking container status..."
docker-compose ps

if [ $? -eq 0 ]; then
    echo "Application started successfully!"
    echo "Access the application at https://localhost"
else
    echo "Error: Some containers failed to start properly"
    docker-compose logs
    exit 1
fi