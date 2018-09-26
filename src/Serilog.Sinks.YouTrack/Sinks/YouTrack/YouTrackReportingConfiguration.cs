using System;
using System.Collections.Generic;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.YouTrack.Services;

namespace Serilog.Sinks.YouTrack
{
    internal sealed class YouTrackReportingConfiguration : IYouTrackReportingConfigurationExpressions
    {
        public const string SummaryTemplate = "[{Level}] {Message}";
        public const string DescriptionTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";
        
        public string Project { get; private set; }        
        public ITextFormatter SummaryFormatter { get; private set; }
        public ITextFormatter DescriptionFormatter { get; private set; }       
        public IReadOnlyCollection<Tuple<Func<LogEvent, Uri, Tuple<string, string>>, bool>> IssueCreated { get; private set; }

        private readonly List<Tuple<Func<LogEvent, Uri, Tuple<string, string>>, bool>> onIssueCreated = new List<Tuple<Func<LogEvent, Uri, Tuple<string, string>>, bool>>();

        public IYouTrackReportingConfigurationExpressions UseProject(string project)
        {
            if (string.IsNullOrEmpty(project))
            {
                throw new ArgumentException(nameof(project));
            }

            Project = project;

            IssueCreated = onIssueCreated.AsReadOnly();

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
        
        public IYouTrackReportingConfigurationExpressions OnIssueCreated(Func<LogEvent, Uri, Tuple<string, string>> executeAgainstIssue, bool failSilently = true)
        {
            if (executeAgainstIssue == null)
            {
                throw new ArgumentNullException(nameof(executeAgainstIssue));
            }
             
            onIssueCreated.Add(Tuple.Create(executeAgainstIssue, failSilently));

            return this;
        }

	    public IYouTrackReportingConfigurationExpressions OnIssueCreated(Func<LogEvent, Uri, string> executeAgainstIssue, bool failSilently = true)
	    {
			if (executeAgainstIssue == null)
		    {
			    throw new ArgumentNullException(nameof(executeAgainstIssue));
		    }

		    Tuple<string, string> Partial(LogEvent e, Uri u) => Tuple.Create(executeAgainstIssue(e, u), (string) null);

		    onIssueCreated.Add(Tuple.Create(new Func<LogEvent, Uri, Tuple<string, string>>(Partial), failSilently));

		    return this;
		}

	    public IYouTrackReportingConfigurationExpressions UsePriority(string priority)
	    {
			if (string.IsNullOrEmpty(priority))
			{
				throw new ArgumentException(nameof(priority));
			}

		    return UsePriority(_ => priority);
	    }

		public IYouTrackReportingConfigurationExpressions UsePriority(Func<LogEvent, string> priority)
		{
			if (priority == null)
			{
				throw new ArgumentNullException(nameof(priority));
			}

			return OnIssueCreated((e, u) => $"{YouTrackContracts.PriorityField} {priority(e)}");
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