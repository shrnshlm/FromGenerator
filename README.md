# AI-Powered Form Generator

An intelligent form generation system that uses Claude AI to analyze user input and create contextually appropriate forms. Available in both **monolithic** and **microservices** architectures.

## 🎯 Live Demo
**Frontend**: http://formgenerator-frontend-2025.s3-website-us-west-2.amazonaws.com

## 🏗️ Architecture Overview

This repository provides **two architectural approaches**:

### 🏢 **Monolithic Architecture** (Currently Deployed)
- **Location**: `FromGenerator/`  
- **Technology**: .NET 8 Web API with integrated services
- **Deployment**: AWS Lambda + S3 Static Hosting
- **Best For**: Quick deployment, small teams, rapid prototyping

### 🔗 **Microservices Architecture**
- **Location**: `microservices/`
- **Technology**: .NET 8 distributed services + Ocelot Gateway
- **Deployment**: Docker Compose ready
- **Best For**: Large teams, independent scaling, technology diversity

## 🎨 Frontend
- **Location**: `client-tsx/`
- **Technology**: React 18 + TypeScript + Tailwind CSS
- **Features**: Smart form generation, responsive design, real-time validation
- **Deployment**: AWS S3 Static Website Hosting

## ⚡ Features

- 🤖 **Claude AI Integration** - Intelligent text analysis and intent detection
- 📝 **Smart Form Generation** - Creates contextually appropriate forms
- 🎯 **Multiple Form Types** - Flight booking, hotel reservation, registration, contact
- 📱 **Responsive Design** - Works seamlessly across all devices
- ⚡ **Real-time Validation** - Client-side and server-side validation
- 🚀 **Production Ready** - Deployed and tested on AWS infrastructure

## 🚀 Quick Start

### Option 1: Monolithic (Recommended for getting started)
```bash
# Backend
cd FromGenerator
dotnet run

# Frontend  
cd client-tsx
npm install
npm run dev
```

### Option 2: Microservices
```bash
cd microservices
docker-compose up --build
```

## 🎮 Try It Out

Enter natural language descriptions and watch the AI generate appropriate forms:

- **Flight Booking**: "I want to book a flight to Paris"
- **Hotel Reservation**: "I need a hotel room in Tokyo for next week"  
- **Registration**: "Sign me up for the conference"
- **Contact Form**: "I have a question about your services"

## 🛠️ Technology Stack

### Backend (.NET 8)
- **AI Integration**: Claude 3.5 Sonnet API
- **Web Framework**: ASP.NET Core Web API
- **Cloud**: AWS Lambda, API Gateway, S3
- **Architecture**: Clean Architecture, SOLID principles

### Frontend (React/TypeScript)
- **Framework**: React 18 + TypeScript
- **Styling**: Tailwind CSS
- **Build Tool**: Vite
- **State Management**: React Hooks
- **HTTP Client**: Fetch API with error handling

### DevOps & Deployment
- **Cloud Platform**: AWS (Lambda, S3, API Gateway)
- **CI/CD**: Git-based deployment
- **Containerization**: Docker + Docker Compose
- **API Gateway**: Ocelot (microservices)

## 📁 Project Structure

```
├── FromGenerator/              # 🏢 Monolithic backend (deployed)
│   ├── Controllers/           # API controllers
│   ├── Services/              # Business logic & Claude integration
│   ├── Models/                # Data models & DTOs
│   └── Configuration/         # App configuration
├── client-tsx/                # 🎨 React/TypeScript frontend (deployed)
│   ├── src/components/        # React components
│   ├── src/services/          # API integration
│   └── src/types/             # TypeScript definitions
├── microservices/             # 🔗 Microservices architecture
│   ├── Gateway/               # API Gateway with Ocelot
│   ├── FormGeneration/        # Form generation service
│   ├── FormSubmission/        # Form submission service
│   ├── TextAnalysis/          # Text analysis service
│   ├── Notification/          # Email notification service
│   └── docker-compose.yml     # Container orchestration
└── README.md                  # This file
```

## 🔧 Configuration

### Environment Variables
```bash
# Claude AI API Key (required for AI features)
CLAUDE_API_KEY=your_claude_api_key_here

# Email Configuration (optional)
EMAIL_SMTP_SERVER=smtp.gmail.com
EMAIL_USERNAME=your_email@gmail.com
EMAIL_PASSWORD=your_app_password
```

### Development Setup
1. Clone the repository
2. Set up environment variables
3. Choose your architecture (monolithic or microservices)
4. Follow the Quick Start guide above

## 🚀 Deployment

### Current Production Deployment
- **Frontend**: AWS S3 Static Website
- **Backend**: AWS Lambda with Function URL
- **Domain**: Auto-generated AWS domains

### Alternative Deployment Options
- **Container**: Docker + Docker Compose
- **Cloud**: Azure, Google Cloud Platform
- **On-Premise**: IIS, Linux with reverse proxy

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Claude AI** by Anthropic for intelligent text analysis
- **AWS** for cloud infrastructure
- **React** and **TypeScript** communities for excellent tooling
- **.NET** team for the robust backend framework

---

**Built with ❤️ using Claude AI assistance**