## How

### Install tool migration

```bash
dotnet tool install --global dotnet-ef
```

### Create migration

```bash
dotnet ef migrations add InitialCreate
```

### Update database

```bash
dotnet ef database update
```

### Rollback migration

```bash
dotnet ef database update 0
```

### Remove migration

```bash
dotnet ef migrations remove
```

### List migration

```bash
dotnet ef migrations list
```
