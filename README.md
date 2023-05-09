# CMS

Content management system.

## PostgreSQL

Execute the following command to create a PostgreSQL Docker image. You can generate secure passwords [here](https://randompasswordgenerator.com/).

`docker run --name cms-postgres -e POSTGRES_USER=cms -e POSTGRES_PASSWORD=<POSTGRES_PASSWORD> -p 5433:5432 -d postgres`

## User Secrets

Copy the contents of the `secrets.example.json` in your user secrets (right-click the `Logitar.Cms` project, then click `Manage User Secrets`).

Replace the following variables:

- `<POSTGRES_PASSWORD>`: the password you assigned to your PostgreSQL Docker image.
- `<LOCAL_IP_ADDRESS>`: your local IP address (ex.: `192.168.X.Y`).
