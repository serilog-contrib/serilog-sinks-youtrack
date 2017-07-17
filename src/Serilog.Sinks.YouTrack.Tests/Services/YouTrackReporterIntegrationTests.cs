using System;
using System.Threading.Tasks;
using Serilog.Sinks.YouTrack.Services;
using Serilog.Sinks.YouTrack.Tests.Harness;
using Xunit;

namespace Serilog.Sinks.YouTrack.Tests.Services
{
    public sealed class YouTrackReporterIntegrationTests : IntegrationTest, IDisposable
    {
        private readonly WrappedYouTrackReporter sut;

        public YouTrackReporterIntegrationTests()
        {
            sut = Reporter();
        }
        
        [Theory]
        [InlineData("PLAYGROUND", "Test Issue", "Test Body", "Bug")]
        public async Task CanReportIssue(string project, string summary, string description, string type)
        {            
            await sut.CreateIssue(project, summary, description, type);            

            Assert.NotEmpty(sut.CreatedIssues);
        }

        [Fact]
        public void CanAuthImmediately()
        {
            var e = Assert.Throws<AggregateException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new YouTrackReporter("admin", "admin", new Uri(YouTrackCredentials["host"]), true);
            });

            Assert.True(e.ToString().IndexOf("403", StringComparison.Ordinal) > -1);
        }

        public void Dispose()
        {
            sut.Wrapped.Dispose();
        }
    }
}