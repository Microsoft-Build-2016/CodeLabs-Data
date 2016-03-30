using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace Common
{
    /// <summary>
    /// Secure string extension methods for converting string to and from.
    /// </summary>
    public static class SecureStringExtensionMethods
    {
        /// <summary>
        /// To the secure string.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Need to be returned. Disposed up a level")]
        public static SecureString ToSecureString(this string input)
        {
            Argument.CheckIfNull(input, "input");

            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        /// <summary>
        /// To the insecure string.
        /// </summary>
        public static string ToInsecureString(this SecureString input)
        {
            Argument.CheckIfNull(input, "input");

            string returnValue;

            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);

            }
            return returnValue;
        }
    }
}