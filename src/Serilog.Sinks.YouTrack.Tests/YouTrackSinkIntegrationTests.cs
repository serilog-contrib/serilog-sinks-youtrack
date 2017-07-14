using System;
using System.Linq;
using Serilog.Core;
using Serilog.Sinks.YouTrack.Tests.Harness;
using Xunit;

namespace Serilog.Sinks.YouTrack.Tests
{    
    public sealed class YouTrackSinkIntegrationTests : IntegrationTest, IDisposable
    {
        private readonly Logger sut;

        public YouTrackSinkIntegrationTests()
        {
            sut = new LoggerConfiguration()
                .WriteTo.YouTrack(Reporter, c => c.
                UseProject("PLAYGROUND").
                UseIssueType(e => e.Exception != null ? "Bug" : "Task").
                FormatSummaryWith("{Timestamp:yyyy-MM-dd} [{Level}] {Message}").
                FormatDescriptionWith("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")).CreateLogger();
        }

        [Fact]
        public void CanLogException()
        {
            // ReSharper disable once RedundantExplicitParamsArrayCreation
            sut.Error(new AggregateException(new Exception[] { new ArgumentException("First"), new InvalidOperationException("Second", new InvalidOperationException("Nested"))}), "Err");
        }

        [Fact]
        public void CanLogMassiveException()
        {
            // ReSharper disable once RedundantExplicitParamsArrayCreation
            var e = Enumerable.Range(0, 50);
            sut.Fatal(new AggregateException(e.Select(x => new InvalidOperationException($"Exception {x}")).ToList()), string.Join(Environment.NewLine, e.Select(x => $"Err {x}")));
        }

        [Fact]
        public void CanLogInformation()
        {
            sut.Information("Info");
        }

        [Fact]
        public void CanUseOverloadsToCreateLoggerFromCredentials()
        {
            using (var mySut = new LoggerConfiguration()
                .WriteTo.YouTrack(new Uri(YouTrackCredentials["host"]), YouTrackCredentials["login"], YouTrackCredentials["password"], "PLAYGROUND").CreateLogger())
            {
                mySut.Error(new Exception("Test"), "test");
            }
        }
        
        public void Dispose()
        {
            sut.Dispose();
        }
    }
}