using System;
using Serilog.Sinks.YouTrack.Tests.Harness;
using Xunit;

namespace Serilog.Sinks.YouTrack.Tests
{
    public class YouTrackSinkTests
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
    }
}