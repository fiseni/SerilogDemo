# Serilog Configuration

This repository contains a sample Serilog configuration for ASP.NET Core applications. It acts as a reminder how to properly configure various sinks. I tend to read the Serilog configuration from `appsettings.json` files, it's a more flexible solution.

## File Sink

I don't like the default output template. I prefer the following customizations.

```json
{
  "Name": "File",
  "Args": {
    "path": "./Logs/log-.txt",
    "rollingInterval": "Day",
    "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] | <{ThreadId}> | UserId:{UserId} | {TraceId} | {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}"
  }
}
```

## Seq Sink

For the Seq sink, the minimal configuration is as follows.

```json
{
  "Name": "Seq",
  "Args": {
    "serverUrl": "http://localhost:5341",
    "apiKey": "my_seq_key"
  }
}
```

## MS SQL Sink

Create the following table in the target database.

```sql
CREATE TABLE [Logs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Message] nvarchar(max) NULL,
    [MessageTemplate] nvarchar(max) NULL,
    [Level] nvarchar(128) NULL,
    [TimeStamp] datetime NOT NULL,
    [Exception] nvarchar(max) NULL,
    [LogEvent] nvarchar(max) NULL,
    [ProcessName] varchar(100) NULL,
    [UserId] varchar(100) NULL,
    [TraceId] varchar(100) NULL,
    [SpanId] varchar(100) NULL,

    CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ([Id] ASC)
);
```

The configuration is as follows. We'll use `LogEvent` instead of `Properties`.

```json
{
  "Name": "MSSqlServer",
  "Args": {
    "connectionString": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SerilogDemo;Integrated Security=SSPI;Trusted_Connection=True;",
    "restrictedToMinimumLevel": "Information",
    "sinkOptionsSection": {
      "tableName": "Logs"
    },
    "columnOptionsSection": {
      "addStandardColumns": [ "LogEvent", "TraceId", "SpanId" ],
      "removeStandardColumns": [ "Properties" ],
      "additionalColumns": [
        {
          "ColumnName": "ProcessName",
          "DataType": "varchar",
          "DataLength": 100
        },
        {
          "ColumnName": "UserId",
          "DataType": "varchar",
          "DataLength": 100
        }
      ],
      "logEvent": {
        "excludeAdditionalProperties": true,
        "excludeStandardColumns": true
      }
    }
  }
},
```