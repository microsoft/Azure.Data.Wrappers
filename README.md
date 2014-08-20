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
await container.Save("Model", model);

//Get Model
await container.Get<object>("Model");

//Save bytes to Blob
var bytes = new byte[];
await container.Save("Binary", bytes);

//Get Bytes
await container.Get("Binary");
```

### Table Storage
```
var table = new TableStorage("Table", "UseDevelopmentStorage=true");
await table.CreateIfNotExists();

// Store Entity
var entity = new TableEntity();
await table.Insert(entity);

//Query by Partition & Row
table.QueryByPartitionAndRow<Model>("partition", "key");

//Query by Partition
table.QueryByPartition<Model>("partition");

//Query by Row
table.QueryByRow<Model>("key");
```

### Queues
```
```