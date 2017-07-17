using System;
using System.Threading.Tasks;

namespace Serilog.Sinks.YouTrack.Services
{
    /// <summary>
    /// Create new issues in YouTrack
    /// </summary>
    public interface IYouTrackReporter
    {
        /// <summary>
        /// Creates a new issues in YouTrack.
        /// </summary>
        /// <param name="project">YouTrack project ID.</param>
        /// <param name="summary">Issue summary.</param>
        /// <param name="description">Issue description.</param>
        /// <param name="issueType">YouTrack issue type. Otherwise YouTrack default for new issues is used.</param>
        /// <returns>Uri to the created issue.</returns>
        Task<Uri> CreateIssue(string project, string summary, string description, string issueType = null);

        /// <summary>
        /// Execute YouTrack command against issue.
        /// <see href="https://www.jetbrains.com/help/youtrack/incloud/Command-Reference.html">YouTrack Command Reference</see>.
        /// </summary>
        /// <param name="issue">Issue uri from CreateIssue.</param>
        /// <param name="command">Command to execute.</param>
        /// /// <param name="comment">Comment to add.</param>
        /// <returns>Uri of the provided issue.</returns>
        Task<Uri> ExecuteAgainstIssue(Uri issue, string command, string comment = null);
    }
}