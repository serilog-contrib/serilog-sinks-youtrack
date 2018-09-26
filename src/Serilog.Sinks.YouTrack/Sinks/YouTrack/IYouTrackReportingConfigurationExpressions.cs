using System;
using Serilog.Events;

namespace Serilog.Sinks.YouTrack
{
    /// <summary>
    /// Configure reporting parameters such as YouTrack project, issue types and templates.
    /// </summary>
    public interface IYouTrackReportingConfigurationExpressions
    {
        /// <summary>
        /// Specify the project to which issues are assigned. Needs to be configured.
        /// </summary>
        /// <param name="project">YouTrack project ID</param>
        /// <remarks>Needs to be specified</remarks>
        IYouTrackReportingConfigurationExpressions UseProject(string project);
        /// <summary>
        /// Specify issue type to use, e.g. Bug.
        /// </summary>
        /// <param name="issueType">YouTrack issue type.</param>        
        IYouTrackReportingConfigurationExpressions UseIssueType(string issueType);
        /// <summary>
        /// Specify issue type to use based on provided LogEvent.
        /// </summary>
        /// <param name="issueType">Func that returns YouTrack issue type.</param>
        IYouTrackReportingConfigurationExpressions UseIssueType(Func<LogEvent, string> issueType);
        /// <summary>
        /// Customize issue summary formatting.
        /// If not specified, "[{Level}] {Message}" is used.
        /// </summary>
        /// <param name="template">Template to use</param>
        /// <param name="formatProvider">Format provider to use.</param>
        IYouTrackReportingConfigurationExpressions FormatSummaryWith(string template, IFormatProvider formatProvider = null);
        /// <summary>
        /// Customize issue description formatting.
        /// If not specified, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}" is used.
        /// </summary>
        /// <param name="template">Template to use.</param>
        /// <param name="formatProvider">Format provider to use.</param>        
        IYouTrackReportingConfigurationExpressions FormatDescriptionWith(string template, IFormatProvider formatProvider = null);
        /// <summary>
        /// When a new issue is logged in YouTrack, execute any commands against it.
        /// <see href="https://www.jetbrains.com/help/youtrack/incloud/Command-Reference.html">YouTrack Command Reference</see>.
        /// </summary>
        /// <param name="executeAgainstIssue">Func executed with the source LogEvent and issue uri. Should return a tuple of YouTrack command(s) with an optional comment.</param>
        /// <param name="failSilently">If command execution fails, don't retry logging.</param>
        /// <remarks>Can be used to register multiple invocations.</remarks>
        IYouTrackReportingConfigurationExpressions OnIssueCreated(Func<LogEvent, Uri, Tuple<string, string>> executeAgainstIssue, bool failSilently = true);
	    /// <summary>
		/// When a new issue is logged in YouTrack, execute any commands against it.
		/// <see href="https://www.jetbrains.com/help/youtrack/incloud/Command-Reference.html">YouTrack Command Reference</see>.
		/// </summary>
		/// <param name="executeAgainstIssue">Func executed with the source LogEvent and issue uri. Should return YouTrack command(s).</param>
		/// <param name="failSilently">If command execution fails, don't retry logging.</param>
		/// <remarks>Can be used to register multiple invocations.</remarks>
		IYouTrackReportingConfigurationExpressions OnIssueCreated(Func<LogEvent, Uri, string> executeAgainstIssue, bool failSilently = true);
		/// <summary>
		/// Customize issue priority.
		/// </summary>
		/// <param name="priority">Issue priority</param>		
		IYouTrackReportingConfigurationExpressions UsePriority(string priority);
		/// <summary>
		/// Customize issue priority.
		/// </summary>
		/// <param name="priority">Func that returns YouTrack issue priority.</param>		
		IYouTrackReportingConfigurationExpressions UsePriority(Func<LogEvent, string> priority);
	}
}