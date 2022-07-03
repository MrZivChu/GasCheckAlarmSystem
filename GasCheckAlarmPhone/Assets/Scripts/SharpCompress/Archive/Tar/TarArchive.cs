using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Tar;
using SharpCompress.Common.Tar.Headers;
using SharpCompress.IO;
using SharpCompress.Reader;
using SharpCompress.Reader.Tar;
using SharpCompress.Writer.Tar;

namespace SharpCompress.Archive.Tar
{
    public class TarArchive : AbstractWritableArchive<TarArchiveEntry, TarVolume>
    {


        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        public static TarArchive Open(Stream stream)
        {
            stream.CheckNotNull("stream");
            return Open(stream, Options.None);
        }

        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        public static TarArchive Open(Stream stream, Options options)
        {
            stream.CheckNotNull("stream");
            return new TarArchive(stream, options);
        }



        public static bool IsTarFile(Stream stream)
        {
            try
            {
                TarHeader tar = new TarHeader();
                tar.Read(new BinaryReader(stream));
                return tar.Name.Length > 0 && Enum.IsDefined(typeof(EntryType), tar.EntryType);
            }
            catch
            {
            }
            return false;
        }



        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        internal TarArchive(Stream stream, Options options)
            : base(ArchiveType.Tar, stream.AsEnumerable(), options)
        {
        }

        internal TarArchive()
            : base(ArchiveType.Tar)
        {
        }

        protected override IEnumerable<TarVolume> LoadVolumes(IEnumerable<Stream> streams, Options options)
        {
            return new TarVolume(streams.First(), options).AsEnumerable();
        }

        protected override IEnumerable<TarArchiveEntry> LoadEntries(IEnumerable<TarVolume> volumes)
        {
            Stream stream = volumes.Single().Stream;
            TarHeader previousHeader = null;
            foreach (TarHeader header in TarHeaderFactory.ReadHeader(StreamingMode.Seekable, stream))
            {
                if (header != null)
                {
                    if (header.EntryType == EntryType.LongName)
                    {
                        previousHeader = header;
                    }
                    else
                    {
                        if (previousHeader != null)
                        {
                            var entry = new TarArchiveEntry(this, new TarFilePart(previousHeader, stream), CompressionType.None);
                            var memoryStream = new MemoryStream();
                            entry.WriteTo(memoryStream);
                            memoryStream.Position = 0;
                            var bytes = memoryStream.ToArray();
                            header.Name = ArchiveEncoding.Default.GetString(bytes, 0, bytes.Length).TrimNulls();
                            previousHeader = null;
                        }
                        yield return new TarArchiveEntry(this, new TarFilePart(header, stream), CompressionType.None);
                    }
                }
            }
        }

        public static TarArchive Create()
        {
            return new TarArchive();
        }

        protected override TarArchiveEntry CreateEntry(string filePath, Stream source,
            long size, DateTime? modified, bool closeStream)
        {
            return new TarWritableArchiveEntry(this, source, CompressionType.Unknown, filePath, size, modified, closeStream);
        }

        protected override void SaveTo(Stream stream, CompressionInfo compressionInfo,
            IEnumerable<TarArchiveEntry> oldEntries,
            IEnumerable<TarArchiveEntry> newEntries)
        {
            using (var writer = new TarWriter(stream, compressionInfo))
            {
                foreach (var entry in oldEntries.Concat(newEntries)
                    .Where(x => !x.IsDirectory))
                {
                    using (var entryStream = entry.OpenEntryStream())
                    {
                        writer.Write(entry.FilePath, entryStream, entry.LastModifiedTime, entry.Size);
                    }
                }
            }
        }

        protected override IReader CreateReaderForSolidExtraction()
        {
            var stream = Volumes.Single().Stream;
            stream.Position = 0;
            return TarReader.Open(stream);
        }
    }
}