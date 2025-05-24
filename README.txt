Commands

1. Open terminal
Go to your project root folder:
cd path_to_your_project


2. Add a migration
  comand - dotnet ef migrations add MigrationName --project Persistance --startup-project API --context MainDataContext


3. Apply migration to database
dotnet ef database update --project Persistance --startup-project API --context MainDataContext
Notes
--project — the project where migrations are stored (Persistance)
--startup-project — the project that runs the application (API)
--context — the DbContext name (MainDataContext)


4. Set User-Secrets

You run the dotnet user-secrets … commands in a terminal in the root folder of your project (where the .csproj file is located).
Open the console (CLI/PowerShell/VS Code terminal) or the Package Manager Console in Visual Studio (Tools → NuGet Package Manager → Package Manager Console).

  comand - dotnet user-secrets init
  comand - dotnet user-secrets set "Authentication:Google:ClientId" "<ClientId>"
  comand - dotnet user-secrets set "Authentication:Google:ClientSecret" "<ClientSecret>"

