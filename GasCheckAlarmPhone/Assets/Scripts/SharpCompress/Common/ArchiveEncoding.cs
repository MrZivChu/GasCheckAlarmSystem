using System.Globalization;
using System.Text;

namespace SharpCompress.Common
{
    public class ArchiveEncoding
    {
        /// <summary>
        /// Default encoding to use when archive format doesn't specify one.
        /// </summary>
        public static Encoding Default;

        /// <summary>
        /// Encoding used by encryption schemes which don't comply with RFC 2898.
        /// </summary>
        public static Encoding Password;

        static ArchiveEncoding()
        {
            Default = Encoding.UTF8;
            Password = Encoding.UTF8;

        }
    }
}
