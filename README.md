# Amar School — Tutor Management System

An ASP.NET Core MVC (.NET 8) application for managing tutors, trainers and students, backed by SQL Server.

- **Guests** see the landing page and can register as a Student or Tutor.
- **Trainers** (added only by the admin) train tutors, mark trainings complete and issue certificates.
- **Tutors** pay for a training, get certified, then offer that course to students and upload materials.
- **Students** pay for a course taught by a certified tutor, download materials and rate the tutor (1–5 stars) after completing it.
- **Admin** manages users, groups, courses and coupons, and is the only one who can see payments and the activity log.

Payments are a demo (no real gateway). Coupons can be **percentage** or **fixed amount** and have an expiry date, a usage limit and an on/off switch.

## Project structure

| Project | Purpose |
|---|---|
| `ProjectShihab.Entities` | EF Core entity classes and constants (roles, statuses) |
| `ProjectShihab.Data` | `AsDbContext` and EF Core migrations |
| `ProjectShihab.Repos` | Business logic / data access (one repo per area), seeders |
| `ProjectShihab.Shared` | `Result<T>` and `CurrentUserHelper` |
| `ProjectShihab.Models` | View models (login, register, checkout, rating, ...) |
| `ProjectShihab.Web` | MVC web app: the main application |
| `ProjectShihab.Api` | REST API with Swagger over the same repos |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or newer)
- SQL Server (Express is fine) and SQL Server Management Studio
- Optional, for migration commands: `dotnet tool install --global dotnet-ef`

## Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/shihab1400/dotNet_Tutor_MS.git
   cd dotNet_Tutor_MS
   ```

2. **Create the database.** In SSMS create an empty database named `ADNProject`.

3. **Set the connection string.** Edit `ConnectionStrings:AsDbContext` in both
   `ProjectShihab.Web/appsettings.json` and `ProjectShihab.Api/appsettings.json`
   (and the `appsettings.Development.json` files) to point at your SQL Server instance:

   ```json
   "ConnectionStrings": {
     "AsDbContext": "Data Source=YOUR-PC\\SQLEXPRESS;Initial Catalog=ADNProject;TrustServerCertificate=True;Integrated Security=True;"
   }
   ```

4. **Run the web app**

   ```bash
   dotnet run --project ProjectShihab.Web --launch-profile https
   ```

   Then open https://localhost:7295.

   On startup the app automatically:
   - applies the EF Core migrations (creates all tables),
   - seeds the admin account plus the base groups and courses,
   - seeds demo data once (trainers, tutors, students, trainings, certificates, courses,
     enrollments, payments, coupons, ratings, materials and log entries).

   To create the tables manually instead, run:

   ```bash
   dotnet ef database update --project ProjectShihab.Data --startup-project ProjectShihab.Web
   ```

5. **(Optional) Run the API**

   ```bash
   dotnet run --project ProjectShihab.Api --launch-profile https
   ```

   Swagger UI is at https://localhost:7056/swagger.

## Demo logins

| Role | Email | Password |
|---|---|---|
| Admin | `admin@adn.com` | `Admin@123` |
| Trainer | `kamrul.hasan@trainer.com` | `Pass@123` |
| Tutor | `rafiq.islam@tutor.com` | `Pass@123` |
| Student | `ayan.mahmud@student.com` | `Pass@123` |

Every demo user's password is `Pass@123`. Other accounts follow the same pattern, for example
`nusrat.jahan@trainer.com`, `sadia.rahman@tutor.com` and `nabila.haque@student.com`.
The full list is on the admin's **Users** page.

Demo coupon codes include `WELCOME10` (10%), `EID25` (25%), `SAVE500` (৳500 off) and `TEACHER15` (15%).
`SUMMER24` is expired and `OLDPROMO` is inactive, for testing rejection.

> Change the admin password before using this anywhere other than a local demo.

## Configuration

| Setting (`ProjectShihab.Web/appsettings.json`) | Default | Effect |
|---|---|---|
| `ConnectionStrings:AsDbContext` | `ADNProject` on `SHIHAB-PC\SQLEXPRESS02` | Database to use |
| `SeedDemoData` | `true` (when absent) | Add `"SeedDemoData": false` to skip the demo data on a fresh database |

Uploaded materials are stored in `ProjectShihab.Web/wwwroot/uploads/` (ignored by git) and served
through the app, so only enrolled users, the owner and the admin can download them.

## Troubleshooting

- **`Failed to bind to address ... access permissions` (socket error 10013):** Windows has reserved that port.
  Change the `applicationUrl` ports in the project's `Properties/launchSettings.json`.
- **Cannot connect to SQL Server:** check the instance name in the connection string
  (`sqlcmd -S YOUR-PC\SQLEXPRESS -E -Q "select @@version"`) and that the SQL Server service is running.
- **Reset the demo data:** drop and recreate the `ADNProject` database, then start the web app again.
