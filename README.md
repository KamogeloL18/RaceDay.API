# RaceDay API - Programming 2B POE Part 2

A RESTful API for the RaceDay Event Management System, built with ASP.NET Core Web API and Entity Framework Core.

## System Overview

RaceDay is a full-stack web-based event management system designed for the South African road running, walking, and cycling community. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enrol, and track their personal performance history.

## User Roles

| Role | Capabilities |
|------|--------------|
| **Organiser** | Create, update, and delete events; manage categories; view enrolments; capture participant results |
| **Participant** | Browse events; enrol in events; view own enrolments; track personal results |

## Tech Stack

- **Framework:** ASP.NET Core Web API (.NET 8)
- **Database:** SQL Server with Entity Framework Core (Code-First)
- **Authentication:** Session-based authentication with BCrypt password hashing
- **API Documentation:** Swagger / OpenAPI
- **Testing:** xUnit with EF Core InMemory database
- **CI/CD:** GitHub Actions

## Project Structure
RaceDay-API/
├── .github/workflows/ci.yml # GitHub Actions CI pipeline
├── .gitignore # Git ignore file
├── README.md # This file
├── ci-success.png # CI/CD success screenshot
│
├── RaceDay.API/ # API project
│ ├── RaceDay.API.csproj
│ ├── RaceDay.API.sln
│ ├── Program.cs
│ ├── appsettings.json
│ ├── appsettings.Development.json
│ ├── Controllers/ # API endpoints
│ │ ├── AuthController.cs
│ │ ├── ProfileController.cs
│ │ ├── EventsController.cs
│ │ ├── CategoriesController.cs
│ │ ├── EnrolmentsController.cs
│ │ └── ParticipantsController.cs
│ ├── Models/ # Entity classes
│ │ ├── User.cs
│ │ ├── Participant.cs
│ │ ├── Organiser.cs
│ │ ├── Event.cs
│ │ ├── Category.cs
│ │ ├── Enrolment.cs
│ │ └── Result.cs
│ ├── Data/
│ │ └── ApplicationDbContext.cs
│ ├── Migrations/ # EF Core migrations
│ └── Properties/
│ └── launchSettings.json
│
└── RaceDay.Tests/ # Unit test project
├── RaceDay.Tests.csproj
├── TestSession.cs
├── AuthControllerTests.cs
├── EventsControllerTests.cs
└── EnrolmentsControllerTests.cs

## Setup Instructions

### Prerequisites

- Visual Studio 2022
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/KamogeloL18/RaceDay-API.git
   cd RaceDay-API
   ## CI/CD Status

![CI/CD Build](https://github.com/KamogeloL
18/RaceDay-API/actions/workflows/ci.yml/badge.svg)

### Successful Build

![CI/CD Success](ci-success.png)

## Video Presentation

[Watch the Part 2 walkthrough](https://youtu.be/YOUR-VIDEO-LINK)

## Testing

Run all tests:
```bash
dotnet test
