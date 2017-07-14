using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Sinks.YouTrack.Services;
using Xunit;

namespace Serilog.Sinks.YouTrack.Tests.Harness
{
    [Trait("Category", "Integration")]
    public abstract class IntegrationTest
    {
        protected IntegrationTest()
        {
            var env = Environment.GetEnvironmentVariable("youtrack-connection");

            if (string.IsNullOrEmpty(env))
            {
                throw new InvalidOperationException("youtrack-connection environment variable not set (e.g. host=https://myendpoint;login=user;password=secret).");
            }

            YouTrackCredentials = env.Split(';').Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);

            Reporter = new YouTrackReporter(YouTrackCredentials["login"], YouTrackCredentials["password"].ToSecureString(),
                new Uri(YouTrackCredentials["host"]));
        }

        public readonly IDictionary<string, string> YouTrackCredentials;

        public readonly YouTrackReporter Reporter;
    }
}