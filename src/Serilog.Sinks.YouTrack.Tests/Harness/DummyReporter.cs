using System;
using System.Threading.Tasks;
using Serilog.Sinks.YouTrack.Services;

namespace Serilog.Sinks.YouTrack.Tests.Harness
{
    public sealed class DummyReporter : IYouTrackReporter, IDisposable
    {
        private readonly Action<string, string, string, string> onIssueCreate;
        private readonly Action<Uri, string, string> onExecuteAgainstIssue;

        public DummyReporter(Action<string, string, string, string> onIssueCreate = null, Action<Uri, string, string> onExecuteAgainstIssue = null)
        {
            this.onIssueCreate = onIssueCreate;
            this.onExecuteAgainstIssue = onExecuteAgainstIssue;
        }

        public Task<Uri> CreateIssue(string project, string summary, string description, string issueType = null)
        {
            onIssueCreate?.Invoke(project, summary, description, issueType);
            return Task.FromResult(new Uri("uri:none"));
        }

        public Task<Uri> ExecuteAgainstIssue(Uri issue, string command, string comment = null)
        {
            onExecuteAgainstIssue?.Invoke(issue, command, comment);
            return Task.FromResult(issue);
        }

        public void Dispose()
        {
        }
    }
}