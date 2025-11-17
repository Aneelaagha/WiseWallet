# WiseWallet – Subscription Insights Dashboard

WiseWallet is a full-stack web application that helps users understand and manage their recurring subscriptions.

## Features

- Track recurring subscriptions (merchant, amount, billing interval, status)
- Detect price increases on existing subscriptions
- Calculate normalized monthly spend and annualized spend
- Highlight upcoming renewals within 30 days
- Seed sample data for demos
- PostgreSQL-backed ASP.NET Core Web API
- Clean HTML/CSS dashboard frontend (no framework required)
- Docker setup for running PostgreSQL + API + frontend together
- GitHub Actions CI pipeline (builds backend & frontend, builds Docker images)

## Tech Stack

- **Backend:** C#, ASP.NET Core 8 minimal APIs, Entity Framework Core, PostgreSQL
- **Frontend:** HTML, CSS, vanilla JavaScript
- **Database:** PostgreSQL
- **DevOps:** Docker, docker-compose, GitHub Actions

## Running locally (without Docker)

1. Ensure you have:
   - .NET 8 SDK
   - PostgreSQL running locally

2. Update `backend/appsettings.Development.json` with your PostgreSQL connection string.

3. From the `backend/` folder:

```bash
dotnet restore
dotnet run
```

API will run on `http://localhost:5000`.

4. Open `frontend/index.html` in your browser and make sure the `API_BASE_URL` matches the backend URL.

## Running with Docker

From the project root:

```bash
docker-compose up --build
```

This will start:
- PostgreSQL
- WiseWallet API
- Frontend static server

Frontend will be available on `http://localhost:4173`.
API will be available on `http://localhost:5000`.

You can push this project directly to GitHub as a portfolio-ready full-stack app.
