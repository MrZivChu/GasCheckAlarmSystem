using System;
using System.IO;
using SharpCompress.Archive.GZip;
using SharpCompress.Archive.Rar;
using SharpCompress.Archive.SevenZip;
using SharpCompress.Archive.Tar;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;

namespace SharpCompress.Archive
{
    public class ArchiveFactory
    {
        /// <summary>
        /// Opens an Archive for random access
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IArchive Open(Stream stream, Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            if (!stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("Stream should be readable and seekable");
            }

            if (ZipArchive.IsZipFile(stream, null))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return ZipArchive.Open(stream, options, null);
            }
            stream.Seek(0, SeekOrigin.Begin);
            if (RarArchive.IsRarFile(stream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return RarArchive.Open(stream, options);
            }
            stream.Seek(0, SeekOrigin.Begin);
            if (TarArchive.IsTarFile(stream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return TarArchive.Open(stream, options);
            }
            stream.Seek(0, SeekOrigin.Begin);
            if (SevenZipArchive.IsSevenZipFile(stream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return SevenZipArchive.Open(stream, options);
            }
            stream.Seek(0, SeekOrigin.Begin);
            if (GZipArchive.IsGZipFile(stream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return GZipArchive.Open(stream, options);
            }
            throw new InvalidOperationException("Cannot determine compressed stream type.");
        }

        public static IArchive Create(ArchiveType type)
        {
            switch (type)
            {
                case ArchiveType.Zip:
                    {
                        return ZipArchive.Create();
                    }
                case ArchiveType.Tar:
                    {
                        return TarArchive.Create();
                    }
                default:
                    {
                        throw new NotSupportedException("Cannot create Archives of type: " + type);
                    }
            }
        }

    }
}
