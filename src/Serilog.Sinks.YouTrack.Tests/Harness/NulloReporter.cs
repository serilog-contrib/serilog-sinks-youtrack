using System;
using System.Threading.Tasks;
using Serilog.Sinks.YouTrack.Services;

namespace Serilog.Sinks.YouTrack.Tests.Harness
{
    public sealed class NulloReporter : IYouTrackReporter
    {
        public Task<Uri> CreateIssue(string project, string summary, string description, string issueType = null)
        {
            return Task.FromResult(new Uri("uri:none"));
        }
    }
}