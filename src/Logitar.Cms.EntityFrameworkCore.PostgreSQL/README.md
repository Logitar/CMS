# Logitar.Cms.EntityFrameworkCore.PostgreSQL

...

## Migrations

This project is setup to use migrations. The following commands must be executed in the solution directory.

### Add a migration

To create a new migration, execute the following command. Do not forget to provide a name for your migration!

`dotnet ef migrations add <YOUR_MIGRATION_NAME> --context CmsContext --project src/Logitar.Cms.EntityFrameworkCore.PostgreSQL --startup-project src/Logitar.Cms`
