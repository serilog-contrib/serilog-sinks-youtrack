using System;
using Serilog.Events;
using Serilog.Sinks.YouTrack.Tests.Harness;
using Xunit;

namespace Serilog.Sinks.YouTrack.Tests
{
    public sealed class YouTrackSinkTests
    {
        [Fact]
        public void ProjectNeedsToBeSpecified()
        {
            Assert.Throws<ArgumentException>(() => new LoggerConfiguration()
                .WriteTo.YouTrack(new NulloReporter(), c => { }).CreateLogger());
        }

        [Fact]
        public void ReporterNeedsToBeSpecified()
        {
            Assert.Throws<ArgumentNullException>(() => new LoggerConfiguration()
                .WriteTo.YouTrack(null, c => { }).CreateLogger());
        }

        [Fact]
        public void ProjectPropagatesToReporter()
        {
            var project = string.Empty;
            using (var sut = new LoggerConfiguration()
                .WriteTo.YouTrack(new DummyReporter((s, _, __, ___) => project = s), c =>
                    c.UseProject("abc")).CreateLogger())
            {
                sut.Error(new Exception(""), "efg");                
            }            

            Assert.Equal("abc", project);
        }

        [Fact]
        public void TypePropagatesToReporter()
        {
            var issueType = string.Empty;
            using (var sut = new LoggerConfiguration()
                .WriteTo.YouTrack(new DummyReporter((_, __, ___, s) => issueType = s), c =>
                    c.UseProject("abc").UseIssueType(_ => "Feature")).CreateLogger())
            {
                sut.Error(new Exception(""), "efg");                
            }

            Assert.Equal("Feature", issueType);
        }

        [Fact]
        public void CommandAndCommentPropagatesToReporter()
        {
            var issueCommand = string.Empty;
            var issueComment = string.Empty;

            using (var sut = new LoggerConfiguration()
                .WriteTo.YouTrack(new DummyReporter(onExecuteAgainstIssue: (_, cmd, comment) =>
            {
                issueCommand = cmd;
                issueComment = comment;
            }), c =>
                    c.UseProject("abc").OnIssueCreated((e, uri) => Tuple.Create("Priority Major", "Bump priority"))).CreateLogger())
            {
                sut.Error(new Exception(""), "efg");                
            }

            Assert.Equal("Priority Major", issueCommand);
            Assert.Equal("Bump priority", issueComment);
        }

	    [Fact]
	    public void PriorityPropagatesToReporter()
	    {
		    var issueCommand = string.Empty;
		    using (var sut = new LoggerConfiguration()
			    .WriteTo.YouTrack(new DummyReporter(onExecuteAgainstIssue: (_, cmd, comment) =>
			    {
				    issueCommand = cmd;
			    }), c =>
				    c.UseProject("abc").UsePriority("Major")).CreateLogger())
		    {
			    sut.Error(new Exception(""), "efg");
		    }

		    Assert.Equal("Priority Major", issueCommand);		    
	    }


		[Fact]
        public void SinkRespectsConfiguredMinimumErrorLevel()
        {
            var nIssues = 0;

            using (var sut = new LoggerConfiguration()
                .WriteTo.YouTrack(new DummyReporter((s, s1, arg3, arg4) => nIssues += 1), c =>
                    c.UseProject("abc"), restrictedToMinimumLevel: LogEventLevel.Error).CreateLogger())
            {
                sut.Information(new Exception(""), "fatal");
                sut.Error(new Exception(""), "nonfatal");                
            }

            Assert.Equal(1, nIssues);
        }
    }
}