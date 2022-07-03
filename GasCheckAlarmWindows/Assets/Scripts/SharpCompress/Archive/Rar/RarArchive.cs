using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.Compressor.Rar;
using SharpCompress.IO;
using SharpCompress.Reader;
using SharpCompress.Reader.Rar;

namespace SharpCompress.Archive.Rar
{
    public class RarArchive : AbstractArchive<RarArchiveEntry, RarVolume>
    {
        private readonly Unpack unpack = new Unpack();

        internal Unpack Unpack
        {
            get
            {
                return unpack;
            }
        }



        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="options"></param>
        internal RarArchive(IEnumerable<Stream> streams, Options options)
            : base(ArchiveType.Rar, streams, options)
        {
        }

        protected override IEnumerable<RarArchiveEntry> LoadEntries(IEnumerable<RarVolume> volumes)
        {
            return RarArchiveEntryFactory.GetEntries(this, volumes);
        }

        protected override IEnumerable<RarVolume> LoadVolumes(IEnumerable<Stream> streams, Options options)
        {
            return RarArchiveVolumeFactory.GetParts(streams, options);
        }

        protected override IReader CreateReaderForSolidExtraction()
        {
            var stream = Volumes.First().Stream;
            stream.Position = 0;
            return RarReader.Open(stream);
        }

        public override bool IsSolid
        {
            get
            {
                return Volumes.First().IsSolidArchive;
            }
        }

        #region Creation


        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        public static RarArchive Open(Stream stream)
        {
            stream.CheckNotNull("stream");
            return Open(stream.AsEnumerable());
        }

        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        public static RarArchive Open(Stream stream, Options options)
        {
            stream.CheckNotNull("stream");
            return Open(stream.AsEnumerable(), options);
        }

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="streams"></param>
        public static RarArchive Open(IEnumerable<Stream> streams)
        {
            streams.CheckNotNull("streams");
            return new RarArchive(streams, Options.KeepStreamsOpen);
        }

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="options"></param>
        public static RarArchive Open(IEnumerable<Stream> streams, Options options)
        {
            streams.CheckNotNull("streams");
            return new RarArchive(streams, options);
        }

        public static bool IsRarFile(Stream stream)
        {
            return IsRarFile(stream, Options.None);
        }

        public static bool IsRarFile(Stream stream, Options options)
        {
            try
            {
                var headerFactory = new RarHeaderFactory(StreamingMode.Seekable, options);
                RarHeader header = headerFactory.ReadHeaders(stream).FirstOrDefault();
                if (header == null)
                {
                    return false;
                }
                return Enum.IsDefined(typeof(HeaderType), header.HeaderType);
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}