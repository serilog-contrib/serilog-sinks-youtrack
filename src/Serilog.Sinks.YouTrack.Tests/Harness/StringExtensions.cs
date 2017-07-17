using System.Security;

namespace Serilog.Sinks.YouTrack.Tests.Harness
{
    public static class StringExtensions
    {
        public static SecureString ToSecureString(this string self)
        {
            var knox = new SecureString();
            var chars = self.ToCharArray();
            foreach (var c in chars)
            {
                knox.AppendChar(c);
            }
            return knox;
        }
    }
}