# ⚙️ CodePixel Arena – Backend

This repository contains the **backend implementation** of the CodePixel Arena project.

It provides:
- REST APIs for users, challenges, submissions, and pixels
- Real-time communication (SignalR)
- Business logic and validation
- Integration with the database system
- Code execution and solution evaluation

## Setup

1. Navigate to the backend folder:
   `cd codepixel-backend`
2. Restore packages:
   `dotnet restore`
3. Apply database migrations:
   `dotnet tool restore`
   `dotnet tool run dotnet-ef database update`
   
   If you need to regenerate SQL Server migrations after switching providers, use:
   `dotnet tool run dotnet-ef migrations add InitialCreate`
4. Run the backend:
   `dotnet run`

## Notes

- Uses ASP.NET Core 10 with SQL Server for persistence.
- Keycloak is configured for authentication via OpenID Connect.
- The backend login API exchanges credentials with Keycloak at `/api/auth/login` and returns the Keycloak token response.
- Real-time pixel updates are exposed through SignalR at `/pixelHub`.

## Keycloak setup

In `appsettings.json`, update the `Keycloak` block with your realm authority, `client_id`, and `client_secret`.

Example:

```json
"Keycloak": {
  "Authority": "https://your-keycloak-host/auth/realms/your-realm",
  "ClientId": "codepixel-backend",
  "ClientSecret": "your-client-secret",
  "RequireHttpsMetadata": "false"
}
```

🔗 Frontend repository: [link-to-frontend]
