# Credit Calculator

A full-stack loan calculator application with Angular frontend and .NET Core backend.

## Features

- 🏠 Housing loan calculator
- 🚗 Vehicle loan calculator
- 💰 Personal loan calculator
- 🤖 AI-powered chatbot (Groq/Gemini integration)
- 📊 Calculation history storage
- 📱 Responsive design

## Tech Stack

**Frontend:**
- Angular 20
- TypeScript
- SCSS

**Backend:**
- .NET 8 / ASP.NET Core
- Entity Framework Core
- SQL Server
- Clean Architecture (Domain, Application, Infrastructure)

## Getting Started

### Prerequisites
- Node.js (v18+)
- .NET 8 SDK
- SQL Server

### Frontend Setup
```bash
cd kredi-hesaplama-frontend
npm install
ng serve
```
Navigate to `http://localhost:4200`

### Backend Setup
```bash
cd KrediHesaplamaAPI
dotnet restore
dotnet ef database update
dotnet run
```
API runs on `https://localhost:7xxx`

## API Endpoints

- `/api/hesaplama` - Loan calculations
- `/api/krediurunu` - Loan products
- `/api/groq` - AI chatbot

## Project Structure

```
Credit-Calculator/
├── kredi-hesaplama-frontend/    # Angular app
│   ├── src/app/components/      # UI components
│   ├── src/app/services/        # API services
│   └── src/app/models/          # Data models
│
└── KrediHesaplamaAPI/           # .NET API
    ├── KrediHesaplama.Domain/   # Domain models
    ├── KrediHesaplama.Application/  # Business logic
    └── KrediHesaplama.Infrastructure/  # Data access
```

## License

MIT
