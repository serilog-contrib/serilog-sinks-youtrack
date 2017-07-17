using System;
using Serilog.Sinks.PeriodicBatching;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.YouTrack.Services;
// ReSharper disable InvertIf

namespace Serilog.Sinks.YouTrack
{
    /// <summary>
    /// Send off log events to YouTrack
    /// </summary>
    public sealed class YouTrackSink : PeriodicBatchingSink
    {
        private readonly IYouTrackReporter reporter;
        private readonly YouTrackReportingConfiguration configuration;

        /// <summary>
        /// Sink that passes messages to YouTrack.
        /// </summary>
        /// <param name="reporter">Reporter to pass off messages to YouTrack.</param>
        /// <param name="configure">Configure reporting parameters such as YouTrack project, issue types and templates.</param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        public YouTrackSink(IYouTrackReporter reporter, Action<IYouTrackReportingConfigurationExpressions> configure, int batchSizeLimit, TimeSpan period) : base(batchSizeLimit, period)
        {            
            this.reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            configuration = new YouTrackReportingConfiguration();
            configure(configuration);
            configuration.Build();
        }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="events">Events to log</param>        
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            foreach (var e in events)
            {
                using (var writerSummary = new StringWriter())
                using (var writerDescription = new StringWriter())                
                {
                    configuration.SummaryFormatter.Format(e, writerSummary);
                    configuration.DescriptionFormatter.Format(e, writerDescription);
                    
                    var issue = await reporter.CreateIssue(configuration.Project, writerSummary.ToString(), writerDescription.ToString(), configuration.IssueTypeResolver(e));

                    SelfLog.WriteLine($"Created issue {issue}");

                    if (configuration.IssueCreated.Count > 0)
                    {
                        foreach (var n in configuration.IssueCreated)
                        {                            
                            try
                            {
                                var cmd = n.Item1(e, issue);
                                await reporter.ExecuteAgainstIssue(issue, cmd.Item1, cmd.Item2).ConfigureAwait(false);
                            } catch (Exception ex) when (n.Item2)
                            {
                                SelfLog.WriteLine(ex.Message);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dispose any IDisposable IYouTrackReporter implementations
        /// </summary>        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            var disposable = reporter as IDisposable;
            disposable?.Dispose();
        }
    }
}