# Microservices Architecture

This folder contains the microservices implementation of the Form Generator application, providing a distributed alternative to the monolithic `FromGenerator` service.

## 🏗️ Services Overview

### **Core Services**

| Service | Purpose | Port | Technology |
|---------|---------|------|------------|
| **Gateway** | API Gateway & Routing | 5000 | .NET 8 + Ocelot |
| **FormGeneration** | Form creation & AI processing | 5001 | .NET 8 Web API |
| **FormSubmission** | Form data processing & storage | 5002 | .NET 8 Web API |
| **TextAnalysis** | NLU & text processing | 5003 | .NET 8 Web API |
| **Notification** | Email & notification services | 5004 | .NET 8 Web API |

### **Additional Services**

- **ReactApp1** - Alternative React frontend for microservices

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose

### Running with Docker Compose
```bash
cd microservices
docker-compose up --build
```

### Running Individual Services
```bash
# Start Gateway (API Gateway)
cd Gateway
dotnet run

# Start Form Generation Service
cd FormGeneration
dotnet run

# Start other services as needed...
```

## 🔗 Service Communication

The services communicate through the **Gateway** service which uses Ocelot for:
- Request routing
- Load balancing
- Authentication
- Rate limiting

## 📋 API Endpoints

### Gateway Routes
- `GET /api/forms/*` → FormGeneration Service
- `POST /api/submissions/*` → FormSubmission Service  
- `POST /api/analysis/*` → TextAnalysis Service
- `POST /api/notifications/*` → Notification Service

## 🏛️ Architecture Benefits

- **Scalability** - Scale services independently
- **Technology Diversity** - Use different tech stacks per service
- **Fault Isolation** - Service failures don't affect others
- **Team Autonomy** - Teams can own individual services
- **Deployment Flexibility** - Deploy services independently

## 🔧 Configuration

Each service has its own `appsettings.json` with service-specific configuration:
- Database connections
- External API keys
- Service discovery
- Logging configuration

## 📊 Monitoring & Observability

- Health checks on `/health` endpoint
- Structured logging
- Distributed tracing ready
- Metrics collection points

## 🆚 vs Monolithic Approach

| Aspect | Microservices | Monolithic (`FromGenerator`) |
|--------|---------------|------------------------------|
| Deployment | Complex, per-service | Simple, single deployment |
| Scaling | Per-service scaling | Scale entire application |
| Development | Team per service | Single team |
| Technology | Mixed technologies | Single tech stack |
| Testing | Complex integration | Simpler testing |
| Debugging | Distributed tracing needed | Single process debugging |

Choose the architecture that best fits your team size, requirements, and operational capabilities.