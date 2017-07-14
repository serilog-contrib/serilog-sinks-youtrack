# Serilog.Sinks.YouTrack [![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.YouTrack.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.YouTrack/)
Send log events to [YouTrack](https://www.jetbrains.com/youtrack/ "YouTrack").

**Package** [Serilog.Sinks.YouTrack](https://www.nuget.org/packages/Serilog.Sinks.YouTrack) | **Platforms** .NET 4.5, .NET Standard 1.3

### Getting started

Install the [Serilog.Sinks.YouTrack](https://www.nuget.org/packages/Serilog.Sinks.YouTrack) package from NuGet:

```powershell
Install-Package Serilog.Sinks.YouTrack -Pre
```

Enable & configure the sink through one of the LoggerSinkConfiguration.YouTrack extension methods:

```csharp
var log = new LoggerConfiguration().WriteTo.
    YouTrack(new Uri("https://issue.tracker"), "user", "changeit", "PROJECT").
	CreateLogger();
```

To configure templating & issue types:

```csharp
var log = new LoggerConfiguration().WriteTo.
	YouTrack(new Uri("https://issue.tracker"), "user", "changeit", 
	c => c.
        UseProject("PROJECT").                    
        UseIssueType(e => e.Exception != null ? "Bug" : "Task").
        FormatSummaryWith("{Timestamp:yyyy-MM-dd} [{Level}] {Message}").
        FormatDescriptionWith("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")).
	CreateLogger();
```

Note: issues are not created in batches as YouTrack Import REST API would require low-level update permissions on the reporting account. 

Integration tests (against YouTrack) only run if `youtrack-connection` environment variable is set (e.g. `youtrack-connection=host=https://issue.tracker;login=user;password=changeit`).