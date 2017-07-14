using System;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Serilog.Sinks.YouTrack
{
    internal sealed class YouTrackReportingConfiguration : IYouTrackReportingConfigurationExpressions
    {
        public const string SummaryTemplate = "[{Level}] {Message}";
        public const string DescriptionTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";
        
        public string Project { get; private set; }        
        public ITextFormatter SummaryFormatter { get; private set; }
        public ITextFormatter DescriptionFormatter { get; private set; }
        
        public IYouTrackReportingConfigurationExpressions UseProject(string project)
        {
            if (string.IsNullOrEmpty(project))
            {
                throw new ArgumentException(nameof(project));
            }

            Project = project;

            return this;
        }
        
        public IYouTrackReportingConfigurationExpressions UseIssueType(string issueType)
        {
            if (string.IsNullOrEmpty(issueType))
            {
                throw new ArgumentException(nameof(issueType));
            }

            IssueTypeResolver = _ => issueType;

            return this;
        }
        
        public IYouTrackReportingConfigurationExpressions UseIssueType(Func<LogEvent, string> issueType)
        {
            IssueTypeResolver = issueType ?? throw new ArgumentNullException(nameof(issueType));
            return this;
        }

        public Func<LogEvent, string> IssueTypeResolver { get; set; }
        
        public IYouTrackReportingConfigurationExpressions FormatSummaryWith(string template, IFormatProvider formatProvider = null)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            SummaryFormatter = new MessageTemplateTextFormatter(template, formatProvider);

            return this;
        }
        
        public IYouTrackReportingConfigurationExpressions FormatDescriptionWith(string template, IFormatProvider formatProvider = null)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            DescriptionFormatter = new MessageTemplateTextFormatter(template, formatProvider);

            return this;
        }

        public void Build()
        {
            if (string.IsNullOrEmpty(Project))
            {
                throw new ArgumentException($"Project must be specified via {nameof(IYouTrackReportingConfigurationExpressions.UseProject)}.");
            }

            IssueTypeResolver = IssueTypeResolver ?? (_ => null);
            SummaryFormatter = SummaryFormatter ?? new MessageTemplateTextFormatter(SummaryTemplate, null);
            DescriptionFormatter = DescriptionFormatter ?? new MessageTemplateTextFormatter(DescriptionTemplate, null);
        }
    }
}