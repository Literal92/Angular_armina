dotnet tool install --global dotnet-ef --version 3.1.0
dotnet tool update --global dotnet-ef --version 3.1.0
dotnet build
dotnet ef --startup-project ../shop/ database update --context SQLiteDbContext
pause