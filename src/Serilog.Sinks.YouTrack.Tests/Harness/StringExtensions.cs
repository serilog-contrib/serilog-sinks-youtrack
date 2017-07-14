using System.Security;

namespace Serilog.Sinks.YouTrack.Tests.Harness
{
    public static class StringExtensions
    {
        public static SecureString ToSecureString(this string self)
        {
            SecureString knox = new SecureString();
            char[] chars = self.ToCharArray();
            foreach (char c in chars)
            {
                knox.AppendChar(c);
            }
            return knox;
        }
    }
}