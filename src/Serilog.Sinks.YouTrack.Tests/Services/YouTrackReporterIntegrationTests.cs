using System;
using System.Security;
using System.Threading.Tasks;
using Serilog.Sinks.YouTrack.Services;
using Serilog.Sinks.YouTrack.Tests.Harness;
using Xunit;

namespace Serilog.Sinks.YouTrack.Tests.Services
{
    public sealed class YouTrackReporterIntegrationTests : IntegrationTest
    {
        private readonly YouTrackReporter sut;

        public YouTrackReporterIntegrationTests()
        {            
            sut = Reporter;
        }
        
        [Theory]
        [InlineData("PLAYGROUND", "Test Issue", "Test Body", "Bug")]
        public async Task CanReportIssue(string project, string summary, string description, string type)
        {            
            await sut.CreateIssue(project, summary, description, type);            
        }
        
        [Fact]
        public void MustProvideCredentialsAndEndpoint()
        {
            Assert.Throws<ArgumentException>(() => new YouTrackReporter(null, "abc", new Uri("url:none")));
            Assert.Throws<ArgumentNullException>(() => new YouTrackReporter("abc", (SecureString)null, new Uri("url:none")));
            Assert.Throws<ArgumentNullException>(() => new YouTrackReporter("abc", "abc", null));
        }

        [Fact]
        public void CanAuthImmediately()
        {
            Assert.Throws<AggregateException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new YouTrackReporter("admin", "admin", new Uri(YouTrackCredentials["host"]), true);
            });
        }
    }
}