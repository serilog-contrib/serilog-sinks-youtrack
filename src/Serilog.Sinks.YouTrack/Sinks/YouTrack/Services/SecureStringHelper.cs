using System.Runtime.InteropServices;
using System.Security;
#if NET45
using System;
#endif

namespace Serilog.Sinks.YouTrack.Services
{
    internal static class SecureStringHelper
    {
        public static string ToString(SecureString value)
        {
#if NET45
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
#else
            var ptr = SecureStringMarshal.SecureStringToGlobalAllocUnicode(value);
            try
            {
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
#endif
        }

        public static SecureString ToSecureString(string value)
        {
            var knox = new SecureString();
            var chars = value.ToCharArray();

            foreach (var c in chars)
            {
                knox.AppendChar(c);
            }

            return knox;
        }
    }
}