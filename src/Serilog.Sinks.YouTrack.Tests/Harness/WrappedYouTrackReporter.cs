using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog.Sinks.YouTrack.Services;

namespace Serilog.Sinks.YouTrack.Tests.Harness
{
    public sealed class WrappedYouTrackReporter : IYouTrackReporter
    {
        public readonly YouTrackReporter Wrapped;

        public readonly List<Uri> CreatedIssues = new List<Uri>();
        public readonly List<Uri> Executed = new List<Uri>();

        public WrappedYouTrackReporter(YouTrackReporter wrapped)
        {
            Wrapped = wrapped;
        }

        public async Task<Uri> CreateIssue(string project, string summary, string description, string issueType = null)
        {
            var response = await Wrapped.CreateIssue(project, summary, description, issueType);
            CreatedIssues.Add(response);
            return response;
        }

        public async Task<Uri> ExecuteAgainstIssue(Uri issue, string command, string comment = null)
        {
            var response = await Wrapped.ExecuteAgainstIssue(issue, command, comment);
            Executed.Add(response);
            return response;
        }
    }
}