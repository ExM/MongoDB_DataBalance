# Initialize simple MongoDB shard cluster

Run virtual servers
```
docker-compose up -d
```

Initialize shard cluster
```
.\init.ps1
```

# Restore installed ShardEqualizer tool

```shell
dotnet tool restore
```

# Run DisbalanceDemo

Build
```shell
dotnet build .\Examples\DisbalanceDemo.sln
```

Write many documents to collection disbalance.myjobs

```shell
dotnet run --project .\Examples\DisbalanceDemo\DisbalanceDemo.csproj myjobs 12 34 567
```

ProjectId will be in the range from 12 to 34

567 batches of 1000 elements will be recorded

# Enable sharding

```shell
sh.enableSharding("disbalance");
sh.shardCollection( "disbalance.myjobs", { "projectId": 1 } );
```

# Configuration of ShardEqualizer

Initialize
```shell
dotnet ShardEqualizer config-init -hlocalhost
```
Update
```shell
dotnet ShardEqualizer config-update
```

# Show moving chunks

```shell
dotnet ShardEqualizer balancer
```

# Show current deviation

```shell
dotnet ShardEqualizer deviation -sM --format=md --layouts="default,balance"
```

# Full equalize

```shell
dotnet ShardEqualizer equalize
```
