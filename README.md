# StackGamer
## Database creation
Set the Database project as the startup project, in the Package Manager Console tab select as Default Project: "Database"  and execute the following commands:
```csharp
Add-Migration InitialCreate
Update-Database
```
If the database needs to be updated, the Migrations folder could be deleted before. 
## Configuration variables
Copy example.appsettings.json and rename it to appsettings.json.

Replace BaseUrl, GetProductByIdUrl, CategoriesUrl and optionally LogLevel.
