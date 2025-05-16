---
mode: 'edit'
---

# New Logger Message

## Steps

1. Review the current logger messages and determine the next sequential EventId
1. If the user hasn't provided what they would like to log, ask them
1. Create a new logger message using the `LoggerMessage` attribute:

```csharp
  [LoggerMessage(
      Level = LogLevel.Information,
      EventId = 1,
      Message = "Worker running at: {Time}")]
  public static partial void LogWorkerRunning(
      ILogger logger,
      DateTimeOffset time);
```
