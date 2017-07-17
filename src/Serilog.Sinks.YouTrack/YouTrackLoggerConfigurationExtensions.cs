using System;
using System.Security;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.YouTrack;
using Serilog.Sinks.YouTrack.Services;

namespace Serilog
{
    /// <summary>
    /// Plug YouTrack sink to Serilog.
    /// </summary>
    public static class YouTrackLoggerConfigurationExtensions
    {
        /// <summary>
        /// The default batch size for sending off events to YouTrack.
        /// </summary>
        public const int DefaultBatchPostingLimit = 10;
        /// <summary>
        /// The default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Adds a sink that sends log events to YouTrack.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="youTrackEndpoint">YouTrack base address.</param>
        /// <param name="user">Username that is used to authenticate to YouTrack.</param>
        /// <param name="password">Password that is used to authenticate to YouTrack.</param>
        /// <param name="project">YouTrack project ID identifying the project to which issues are created to.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration YouTrack(this LoggerSinkConfiguration sinkConfiguration, Uri youTrackEndpoint,
            string user, string password, string project)
        {
            return YouTrack(sinkConfiguration, youTrackEndpoint, user, SecureStringHelper.ToSecureString(password), project);
        }

        /// <summary>
        /// Adds a sink that sends log events to YouTrack.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="youTrackEndpoint">YouTrack base address.</param>
        /// <param name="user">Username that is used to authenticate to YouTrack.</param>
        /// <param name="password">Password that is used to authenticate to YouTrack.</param>
        /// <param name="project">YouTrack project ID identifying the project to which issues are created to.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration YouTrack(this LoggerSinkConfiguration sinkConfiguration, Uri youTrackEndpoint, string user, SecureString password, string project)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            var reporter = new YouTrackReporter(user, password, youTrackEndpoint);

            var sink = new YouTrackSink(reporter, c => c.UseProject(project), DefaultBatchPostingLimit, DefaultPeriod);

            return sinkConfiguration.Sink(sink);
        }

        /// <summary>
        /// Adds a sink that sends log events to YouTrack.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="youTrackEndpoint">YouTrack base address.</param>
        /// <param name="user">Username that is used to authenticate to YouTrack.</param>
        /// <param name="password">Password that is used to authenticate to YouTrack.</param>        
        /// <param name="reportingConfiguration">Configure reporting parameters such as YouTrack project, issue types and templates. Project needs to always be configured.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>/// 
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration YouTrack(this LoggerSinkConfiguration sinkConfiguration, Uri youTrackEndpoint,
            string user, string password,
            Action<IYouTrackReportingConfigurationExpressions> reportingConfiguration, LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            return YouTrack(sinkConfiguration, youTrackEndpoint, user, SecureStringHelper.ToSecureString(password),
                reportingConfiguration, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that sends log events to YouTrack.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="youTrackEndpoint">YouTrack base address.</param>
        /// <param name="user">Username that is used to authenticate to YouTrack.</param>
        /// <param name="password">Password that is used to authenticate to YouTrack.</param>        
        /// <param name="reportingConfiguration">Configure reporting parameters such as YouTrack project, issue types and templates. Project needs to always be configured.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration YouTrack(this LoggerSinkConfiguration sinkConfiguration, Uri youTrackEndpoint, string user, SecureString password, Action<IYouTrackReportingConfigurationExpressions> reportingConfiguration, LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            var reporter = new YouTrackReporter(user, password, youTrackEndpoint);

            var sink = new YouTrackSink(reporter, reportingConfiguration, DefaultBatchPostingLimit, DefaultPeriod);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that sends log events to YouTrack.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="reporter">The reporter used to pass off log events to YouTrack.</param>
        /// <param name="reportingConfiguration">Configure reporting parameters such as YouTrack project, issue types and templates. Project needs to always be configured.</param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration YouTrack(this LoggerSinkConfiguration sinkConfiguration, IYouTrackReporter reporter, Action<IYouTrackReportingConfigurationExpressions> reportingConfiguration, int batchSizeLimit = DefaultBatchPostingLimit, TimeSpan? period = null, LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            if (reporter == null)
            {
                throw new ArgumentNullException(nameof(reporter));
            }

            if (reportingConfiguration == null)
            {
                throw new ArgumentNullException(nameof(reportingConfiguration));
            }

            period = period ?? DefaultPeriod;

            var sink = new YouTrackSink(reporter, reportingConfiguration, batchSizeLimit, period.Value);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}