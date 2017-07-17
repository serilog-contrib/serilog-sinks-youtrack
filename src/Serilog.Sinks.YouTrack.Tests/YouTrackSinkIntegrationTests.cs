using System;
using System.Linq;
using System.Text;
using Serilog.Core;
using Serilog.Sinks.YouTrack.Tests.Harness;
using Xunit;

namespace Serilog.Sinks.YouTrack.Tests
{    
    public sealed class YouTrackSinkIntegrationTests : IntegrationTest
    {
        private readonly Func<(Logger log, WrappedYouTrackReporter reporter)> sutFactory;

        public YouTrackSinkIntegrationTests()
        {
            sutFactory = () =>
            {
                var reporter = Reporter();
                return (new LoggerConfiguration()
                    .WriteTo.YouTrack(reporter,
                        c => c.UseProject(Project)
                            .UseIssueType(e => e.Exception != null ? "Bug" : "Task")
                            .OnIssueCreated((e, uri) => Tuple.Create("Priority Major", (string)null))
                            .FormatSummaryWith("{Timestamp:yyyy-MM-dd} [{Level}] {Message}")
                            .FormatDescriptionWith(
                                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"), 1)
                    .CreateLogger(), reporter);
            };
        }

        [Fact]
        public void CanLogException()
        {
            var sut = sutFactory();
            using (var log = sut.log)
            {
                // ReSharper disable once RedundantExplicitParamsArrayCreation            
                log.Error(
                    new AggregateException(new Exception[]
                    {
                        new ArgumentException("First"),
                        new InvalidOperationException("Second", new InvalidOperationException("Nested"))
                    }), "Err");                                
            }
            Assert.NotEmpty(sut.reporter.CreatedIssues);
        }

        [Fact]
        public void CanLogMassiveException()
        {
            var sut = sutFactory();
            using (var log = sut.log)
            {
                // ReSharper disable once RedundantExplicitParamsArrayCreation
                var e = Enumerable.Range(0, 50);
                log.Fatal(
                    new AggregateException(e.Select(x => new InvalidOperationException($"Exception {x}")).ToList()),
                    string.Join(Environment.NewLine, e.Select(x => $"Err {x}")));                
            }
            Assert.NotEmpty(sut.reporter.CreatedIssues);
        }

        [Fact]
        public void CanLogInformation()
        {
            var sut = sutFactory();
            using (var log = sut.log)
            {
                log.Information("Info");
                Log.CloseAndFlush();                
            }
            Assert.NotEmpty(sut.reporter.CreatedIssues);
        }

        [Fact]
        public void CanUseOverloadsToCreateLoggerFromCredentials()
        {
            var selfLog = new StringBuilder();
            Debugging.SelfLog.Enable(s => selfLog.Append(s));
            using (var mySut = new LoggerConfiguration()
                .WriteTo.YouTrack(new Uri(YouTrackCredentials["host"]), YouTrackCredentials["login"], YouTrackCredentials["password"], Project).CreateLogger())
            {
                mySut.Error(new Exception("Test"), "test");                
            }
            Assert.True(selfLog.ToString().IndexOf("Created issue", StringComparison.Ordinal) > -1);
        }

        [Fact]
        public void CanExecuteCommandAgainstIssue()
        {
            var sut = sutFactory();
            using (var log = sut.log)
            {
                log.Error(new Exception("Test"), "test");
                Log.CloseAndFlush();                
            }
            Assert.NotEmpty(sut.reporter.CreatedIssues);
            Assert.NotEmpty(sut.reporter.Executed);
        }
    }
}