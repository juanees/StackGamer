# StackGamer
## Creación de base de datos
En la pestaña Consola del Administrador de paquetes, seleccionar como Proyecto predeterminado: "Database" y ejecutar los siguientes comandos:
```csharp
Add-Migration InitialCreate
Update-Database
 ```
## Seteo de variables de configuración
Copiar example.appsettings.json y cambiarle el nombre a appsettings.json

Remplazar BaseUrl, GetProductByIdUrl y opcionalmente LogLevel
