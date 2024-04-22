# Serilog Demo


## MS SQL Sink

```sql
CREATE TABLE [Logs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Message] nvarchar(max) NULL,
    [MessageTemplate] nvarchar(max) NULL,
    [Level] nvarchar(128) NULL,
    [TimeStamp] datetime NOT NULL,
    [Exception] nvarchar(max) NULL,
    [LogEvent] nvarchar(max) NULL,
    [ProcessName] nvarchar(250) NULL,
    [UserId] nvarchar(250) NULL,
    [TraceId] nvarchar(250) NULL,
    [SpanId] nvarchar(250) NULL,

    CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ([Id] ASC)
);
```