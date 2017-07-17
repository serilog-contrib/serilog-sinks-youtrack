using System;
using System.Security;
using Serilog.Sinks.YouTrack.Services;
using Xunit;

namespace Serilog.Sinks.YouTrack.Tests.Services
{
    public class YouTrackReporterTests
    {
        [Fact]
        public void MustProvideCredentialsAndEndpoint()
        {
            Assert.Throws<ArgumentException>(() => new YouTrackReporter(null, "abc", new Uri("url:none")));
            Assert.Throws<ArgumentNullException>(() => new YouTrackReporter("abc", (SecureString)null, new Uri("url:none")));
            Assert.Throws<ArgumentNullException>(() => new YouTrackReporter("abc", "abc", null));
        }
    }
}