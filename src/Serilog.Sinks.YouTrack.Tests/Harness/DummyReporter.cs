using System;
using System.Threading.Tasks;
using Serilog.Sinks.YouTrack.Services;

namespace Serilog.Sinks.YouTrack.Tests.Harness
{
    public sealed class DummyReporter : IYouTrackReporter
    {
        private readonly Action<string, string, string, string> onCall;

        public DummyReporter(Action<string, string, string, string> onCall)
        {
            this.onCall = onCall;
        }

        public Task<Uri> CreateIssue(string project, string summary, string description, string issueType = null)
        {
            onCall(project, summary, description, issueType);
            return Task.FromResult(new Uri("uri:none"));
        }
    }
}