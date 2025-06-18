#  Meter Reading Upload System
---

##  Features

-  PostgreSQL via Docker Compose
-  ASP.NET Core backend with validation, deduplication, and EF Core
-  React frontend to upload CSV and view processing results
-  Unit & integration tests for meter reading logic

---

## Requirements

- [.NET 7+ SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+ and npm](https://nodejs.org/)
- [Docker + Docker Compose](https://www.docker.com/products/docker-desktop)

---

## Project Structure

```
.
├── docker-compose.yml
├── EnergyChecker/                     # ASP.NET Core Web API
│   ├── Controllers/
│   ├── Services/
│   ├── Program.cs
│   └── appsettings.json
├── frontend/                    # React frontend
│   ├── src/
│   │   └── App.tsx
│   ├── index.html
│   └── main.tsx
└── tests/                       # Backend tests
    └── MeterReadingProcessorTests.cs
```

---

## Step 1: Start PostgreSQL with Docker Compose

We use Docker to host the PostgreSQL database.

### Run the DB

```bash
docker compose up -d
```

Creates:
- Database: `mydb`
- Username: `myuser`
- Password: `mypass`
- Port: `5432`

---

## Step 2: Run the ASP.NET Core Backend

### Run migrations (if needed)

```bash
dotnet ef database update
```

### Run the backend

```bash
dotnet run --project EnergyChecker.csproj
```
---

## Step 3: Run the React Frontend

### Install dependencies

```bash
cd frontend
npm install
```

### ▶️ Start dev server

```bash
npm run dev
```

Then open: [http://localhost:5173](http://localhost:5173)

### Upload form behavior

- Choose a `.csv` file
- Click "Upload"
- Shows response with count of processed & failed records

---

## API Endpoint

### `POST /meter-reading-uploads`

- Accepts: `multipart/form-data`
- Field: `file` (CSV file)
- Example request: Upload a file using a form
- Response:

```json
{
  "processed": 12,
  "failed": 3
}
```
