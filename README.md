# StackGamer
## Database creation
In the Package Manager Console tab, select as Default Project: "Database" and execute the following commands:
```csharp
Add-Migration InitialCreate
Update-Database
```
## Configuration variables
Copy example.appsettings.json and rename it to appsettings.json

Replace BaseUrl, GetProductByIdUrl and optionally LogLevel