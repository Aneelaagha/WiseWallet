🧠💰 WiseWallet – Subscription Insights Dashboard

Modern dashboard for tracking subscriptions, detecting price changes, and managing recurring costs.)




Tech Stack
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET%208-512BD4?logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)
![GitHub Actions](https://img.shields.io/badge/GitHub%20Actions-2088FF?logo=githubactions&logoColor=white)
![HTML5](https://img.shields.io/badge/HTML5-E34F26?logo=html5&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?logo=css3&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?logo=javascript&logoColor=black)


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
