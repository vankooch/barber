## EF Core

### Create Migration

```powershell
dotnet ef migrations add "Initial" --project ..\..\Barber.IoT.Context --startup-project .
dotnet ef --project ..\..\Barber.IoT.Context --startup-project . database update
```