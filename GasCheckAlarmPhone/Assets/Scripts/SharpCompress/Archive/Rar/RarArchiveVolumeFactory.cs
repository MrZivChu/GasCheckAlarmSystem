using System;
using System.Collections.Generic;
using System.IO;

using SharpCompress.Common;
using SharpCompress.Common.Rar;

namespace SharpCompress.Archive.Rar
{
    internal static class RarArchiveVolumeFactory
    {
        internal static IEnumerable<RarVolume> GetParts(IEnumerable<Stream> streams, Options options)
        {
            foreach (Stream s in streams)
            {
                if (!s.CanRead || !s.CanSeek)
                {
                    throw new ArgumentException("Stream is not readable and seekable");
                }
                StreamRarArchiveVolume part = new StreamRarArchiveVolume(s, options);
                yield return part;
            }
        }

    }
}
