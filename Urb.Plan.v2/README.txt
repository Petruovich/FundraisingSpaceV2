Commands

1. Open terminal
Go to your project root folder:
cd path_to_your_project


2. Add a migration
dotnet ef migrations add MigrationName --project Persistance --startup-project API --context MainDataContext


3. Apply migration to database
dotnet ef database update --project Persistance --startup-project API --context MainDataContext
Notes
--project — the project where migrations are stored (Persistance)
--startup-project — the project that runs the application (API)
--context — the DbContext name (MainDataContext)