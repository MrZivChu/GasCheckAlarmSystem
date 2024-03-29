﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.GZip;
using SharpCompress.Reader;
using SharpCompress.Reader.GZip;
using SharpCompress.Writer.GZip;

namespace SharpCompress.Archive.GZip
{
    public class GZipArchive : AbstractWritableArchive<GZipArchiveEntry, GZipVolume>
    {

        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        public static GZipArchive Open(Stream stream)
        {
            stream.CheckNotNull("stream");
            return Open(stream, Options.None);
        }

        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        public static GZipArchive Open(Stream stream, Options options)
        {
            stream.CheckNotNull("stream");
            return new GZipArchive(stream, options);
        }



        public static bool IsGZipFile(Stream stream)
        {
            // read the header on the first read
            byte[] header = new byte[10];
            int n = stream.Read(header, 0, header.Length);

            // workitem 8501: handle edge case (decompress empty stream)
            if (n == 0)
                return false;

            if (n != 10)
                return false;

            if (header[0] != 0x1F || header[1] != 0x8B || header[2] != 8)
                return false;

            return true;
        }

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        internal GZipArchive(Stream stream, Options options)
            : base(ArchiveType.GZip, stream.AsEnumerable(), options)
        {
        }

        internal GZipArchive()
            : base(ArchiveType.GZip)
        {
        }

        public void SaveTo(Stream stream)
        {
            this.SaveTo(stream, CompressionType.GZip);
        }

        protected override GZipArchiveEntry CreateEntry(string filePath, Stream source, long size, DateTime? modified, bool closeStream)
        {
            if (Entries.Any())
            {
                throw new InvalidOperationException("Only one entry is allowed in a GZip Archive");
            }
            return new GZipWritableArchiveEntry(this, source, filePath, size, modified, closeStream);
        }

        protected override void SaveTo(Stream stream, CompressionInfo compressionInfo,
            IEnumerable<GZipArchiveEntry> oldEntries, IEnumerable<GZipArchiveEntry> newEntries)
        {
            if (Entries.Count > 1)
            {
                throw new InvalidOperationException("Only one entry is allowed in a GZip Archive");
            }
            using (var writer = new GZipWriter(stream))
            {
                foreach (var entry in oldEntries.Concat(newEntries)
                    .Where(x => !x.IsDirectory))
                {
                    using (var entryStream = entry.OpenEntryStream())
                    {
                        writer.Write(entry.FilePath, entryStream, entry.LastModifiedTime);
                    }
                }
            }
        }

        protected override IEnumerable<GZipVolume> LoadVolumes(IEnumerable<Stream> streams, Options options)
        {
            return new GZipVolume(streams.First(), options).AsEnumerable();
        }

        protected override IEnumerable<GZipArchiveEntry> LoadEntries(IEnumerable<GZipVolume> volumes)
        {
            Stream stream = volumes.Single().Stream;
            yield return new GZipArchiveEntry(this, new GZipFilePart(stream));
        }

        protected override IReader CreateReaderForSolidExtraction()
        {
            var stream = Volumes.Single().Stream;
            stream.Position = 0;
            return GZipReader.Open(stream);
        }
    }
}
