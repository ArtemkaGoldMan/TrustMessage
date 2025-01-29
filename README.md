# TrustMessage

A secure messaging application with end-to-end encryption and user authentication.

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [Git](https://git-scm.com/downloads)
- OpenSSL (for generating self-signed certificates)

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/ArtemkaGoldMan/TrustMessage.git
   cd TrustMessage
   ```

2. Generate SSL certificates for HTTPS:
   ```bash
   # Create a directory for certificates if it doesn't exist
   mkdir -p TrustMessageWeb

   # Generate self-signed certificates
   openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
     -keyout TrustMessageWeb/key.pem \
     -out TrustMessageWeb/cert.pem \
     -subj "/C=US/ST=State/L=City/O=Organization/CN=localhost"
   ```

3. Make the startup script executable:
   ```bash
   chmod +x run-docker.sh
   ```

4. Start the application:
   ```bash
   ./run-docker.sh
   ```

The application will be available at:
- **HTTPS:** https://localhost
- **HTTP:** http://localhost

---

## Technologies Used

- **Frontend:** React.js (28.8%)
- **Backend:** C# / ASP.NET Core (59.1%)
- **Styling:** CSS (11.7%)
- **Templates:** HTML (0.4%)
- **Database:** Microsoft SQL Server
- **Reverse Proxy:** Nginx

---

## Project Structure

```
TrustMessage/
├── TrustMessageApp/         # Backend C# application
├── TrustMessageWeb/         # Frontend React application
├── nginx/                   # Nginx configuration
├── docker-compose.yml       # Docker Compose configuration
├── Dockerfile.db            # Database Dockerfile
├── setup.sql                # Database initialization script
└── run-docker.sh            # Startup script
```

---

## Development

To rebuild and restart the services:
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

To view logs:
```bash
# All services
docker-compose logs

# Specific service
docker-compose logs [service_name]
# Example: docker-compose logs backend
```

