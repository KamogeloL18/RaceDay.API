# RaceDay Event Management System - POE Parts 1-3

A full-stack web-based event management system for the South African road running, walking, and cycling community. Built with ASP.NET Core Web API, Entity Framework Core, ASP.NET Core MVC, Azure Blob Storage, and Docker.

## System Overview

RaceDay allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enrol, and track their personal performance history.

## User Roles

| Role | Capabilities |
|------|--------------|
| **Organiser** | Create, update, delete events; manage categories; view enrolments; capture results; upload event banners |
| **Participant** | Browse events; enrol in events; view own enrolments and results; update profile |

## Tech Stack

- **Backend:** ASP.NET Core Web API (.NET 9)
- **Frontend:** ASP.NET Core MVC (.NET 9)
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** Session-based with BCrypt password hashing
- **Cloud Storage:** Azure Blob Storage (for event banners)
- **API Docs:** Swagger/OpenAPI
- **Testing:** xUnit
- **DevOps:** Docker, GitHub Actions CI/CD

## Project Structure

- `RaceDay.API/` - The RESTful Web API and Database Context.
- `RaceDay.Tests/` - Unit tests for the API.
- `RaceDay.Web/` - The MVC front-end application.

## Setup Instructions (Local Development)

### 1. API Setup
1. Open the solution in Visual Studio 2022.
2. Ensure the connection string in `RaceDay.API/appsettings.json` points to your local SQL Server.
3. Open the Package Manager Console (Default project: `RaceDay.API`) and run:
   ```powershell
   Update-Database

4.Run the RaceDay.API project (Press F5). Swagger will open.

### 2. MVC Web App Setup
1. Set RaceDay.Web as the startup project (or configure Multiple Startup Projects).

2. Run the application (Press F5). It will communicate with the local API.

### Docker Setup (Part 3 Requirement)
The MVC application is fully containerized.

1. Ensure the API is running locally on port 5004 (http://localhost:5004).

2. Open a terminal and navigate to the MVC project folder:
cd RaceDay.Web

3. Build the Docker image:
docker build -t raceday-web .

4. Run the container, passing the API URL environment variable so the container can reach the host machine:

docker run -d -p 8081:8080 -e "ApiSettings__BaseUrl=http://host.docker.internal:5004" --name raceday-mvc raceday-web

5. Access the application at http://localhost:8081.

### Azure Blob Storage

The API is integrated with Azure Blob Storage for uploading event banner images. Images are uploaded via the API and the resulting URL is stored in the database. The connection string is configured in RaceDay.API/appsettings.json.

CI/CD Status
https://github.com/KamogeloL18/RaceDay.API/actions/workflows/ci.yml/badge.svg

Successful Build
https://ci-success.png/

Testing
Run all tests locally:

bash
dotnet test 
