using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;
using SharpCompress.Compressor.Deflate;
using SharpCompress.Reader;
using SharpCompress.Reader.Zip;
using SharpCompress.Writer.Zip;

namespace SharpCompress.Archive.Zip
{
    public class ZipArchive : AbstractWritableArchive<ZipArchiveEntry, ZipVolume>
    {
        private SeekableZipHeaderFactory headerFactory;

        /// <summary>
        /// Gets or sets the compression level applied to files added to the archive,
        /// if the compression method is set to deflate
        /// </summary>
        public CompressionLevel DeflateCompressionLevel { get; set; }



        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="password"></param>
        public static ZipArchive Open(Stream stream, string password = null)
        {
            stream.CheckNotNull("stream");
            return Open(stream, Options.None, password);
        }

        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <param name="password"></param>
        public static ZipArchive Open(Stream stream, Options options, string password = null)
        {
            stream.CheckNotNull("stream");
            return new ZipArchive(stream, options, password);
        }


        public static bool IsZipFile(Stream stream, string password = null)
        {
            StreamingZipHeaderFactory headerFactory = new StreamingZipHeaderFactory(password);
            try
            {
                ZipHeader header =
                    headerFactory.ReadStreamHeader(stream).FirstOrDefault(x => x.ZipHeaderType != ZipHeaderType.Split);
                if (header == null)
                {
                    return false;
                }
                return Enum.IsDefined(typeof(ZipHeaderType), header.ZipHeaderType);
            }
            catch (CryptographicException)
            {
                return true;
            }
            catch
            {
                return false;
            }
        }



        internal ZipArchive()
            : base(ArchiveType.Zip)
        {
        }

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <param name="password"></param>
        internal ZipArchive(Stream stream, Options options, string password = null)
            : base(ArchiveType.Zip, stream.AsEnumerable(), options)
        {
            headerFactory = new SeekableZipHeaderFactory(password);
        }

        protected override IEnumerable<ZipVolume> LoadVolumes(IEnumerable<Stream> streams, Options options)
        {
            return new ZipVolume(streams.First(), options).AsEnumerable();
        }

        protected override IEnumerable<ZipArchiveEntry> LoadEntries(IEnumerable<ZipVolume> volumes)
        {
            var volume = volumes.Single();
            Stream stream = volumes.Single().Stream;
            foreach (ZipHeader h in headerFactory.ReadSeekableHeader(stream))
            {
                if (h != null)
                {
                    switch (h.ZipHeaderType)
                    {
                        case ZipHeaderType.DirectoryEntry:
                            {
                                yield return new ZipArchiveEntry(this,
                                    new SeekableZipFilePart(headerFactory, h as DirectoryEntryHeader, stream));
                            }
                            break;
                        case ZipHeaderType.DirectoryEnd:
                            {
                                byte[] bytes = (h as DirectoryEndHeader).Comment;
                                volume.Comment = ArchiveEncoding.Default.GetString(bytes, 0, bytes.Length);
                                yield break;
                            }
                    }
                }
            }
        }

        protected override void SaveTo(Stream stream, CompressionInfo compressionInfo,
            IEnumerable<ZipArchiveEntry> oldEntries,
            IEnumerable<ZipArchiveEntry> newEntries)
        {
            using (var writer = new ZipWriter(stream, compressionInfo, string.Empty))
            {
                foreach (var entry in oldEntries.Concat(newEntries)
                    .Where(x => !x.IsDirectory))
                {
                    using(var entryStream = entry.OpenEntryStream())
                    {
                        writer.Write(entry.FilePath, entryStream, entry.LastModifiedTime, string.Empty);
                    }
                }
            }
        }

        protected override ZipArchiveEntry CreateEntry(string filePath, Stream source, long size, DateTime? modified, bool closeStream)
        {
            return new ZipWritableArchiveEntry(this, source, filePath, size, modified, closeStream);
        }

        public static ZipArchive Create()
        {
            return new ZipArchive();
        }

        protected override IReader CreateReaderForSolidExtraction()
        {
            var stream = Volumes.Single().Stream;
            stream.Position = 0;
            return ZipReader.Open(stream);
        }
    }
}