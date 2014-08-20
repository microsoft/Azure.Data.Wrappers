Azure Storage Simplified
- Blob Storage
- Table Storage
- Queues
- Mockable, dependancy injection

### BETA Release

## NuGet
[Add via NuGet](https://www.nuget.org/packages/King.Azure)
```
PM> Install-Package King.Azure
```

## Get Started
### Blob Storage
```
var container = new Container("Data", "UseDevelopmentStorage=true");
await container.CreateIfNotExists();

//Save Json Model to Blob
var model = new object();
container.Save("Model", model);

//Save bytes to Blob
var bytes = new byte[];
container.Save("Binary", bytes);
```

### Table Storage
```
```

### Queues
```
```