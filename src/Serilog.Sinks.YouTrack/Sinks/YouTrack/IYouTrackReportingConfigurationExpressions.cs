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
    }
}