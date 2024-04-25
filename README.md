# Serilog Configuration

This repository contains a sample Serilog configuration for ASP.NET Core applications. It acts as a reminder of how to properly configure various sinks. I prefer reading the Serilog configuration from `appsettings.json` files, being a more flexible solution.

## Running the sample app.

Simply, just run the app. On startup, it will automatically create `SerilogDemo` database in your localdb. The db initializer will create the necessary `Logs` table as well.

## Serilog packages

For this example, we need the following packages.

- Serilog.AspNetCore - It contains Console, Debug, File sinks.
- Serilog.Sinks.MSSqlServer
- Serilog.Sinks.Seq
- Serilog.Enrichers.Environment
- Serilog.Enrichers.Process
- Serilog.Enrichers.Thread
- Serilog.Expressions

## Initial configuration

The default request logging implemented by ASP.NET Core is noisy, with multiple events emitted per request. Serilog provides a separate middleware `UseSerilogRequestLogging` which condenses these into a single event. To silence the default events, we'll override the log level to Warning for the AspNetCore namespaces.

Also, we'll enrich the log events with some additional information.

```json
"Serilog": {
  "Using": [ "Serilog.Sinks.File", "Serilog.Sinks.MSSqlServer", "Serilog.Sinks.Seq", "Serilog.Sinks.Console", "Serilog.Sinks.Debug" ],
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft.AspNetCore.Mvc": "Warning",
      "Microsoft.AspNetCore.Routing": "Warning",
      "Microsoft.AspNetCore.Hosting": "Warning"
    }
  },
  "WriteTo": [
  ],
  "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId", "WithProcessName" ]
},
```

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

## Sub-loggers and Filters

Serilog supports filter expressions through the `Serilog.Expressions` package. But, it's inconvenient that we can't apply them per sink. The override rules can not be applied per sink either.
For example, we can't filter out Information level EF logs for the MSSQLServer sink only. As a workaround, we can define a sub-logger and apply specific filters for it. Refer to [appsettings.WithSubLogger.json](https://github.com/fiseni/SerilogDemo/blob/main/src/SerilogDemo.Web/appsettings.WithSubLogger.json) for an example.

Note: Override rules can not be applied to sub-loggers. That's the reason for using filter expressions.
