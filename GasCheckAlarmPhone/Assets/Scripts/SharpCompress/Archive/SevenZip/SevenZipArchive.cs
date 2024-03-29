﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.SevenZip;
using SharpCompress.IO;
using SharpCompress.Reader;

namespace SharpCompress.Archive.SevenZip
{
    public class SevenZipArchive : AbstractArchive<SevenZipArchiveEntry, SevenZipVolume>
    {
        private SevenZipHeaderFactory factory;


        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        public static SevenZipArchive Open(Stream stream)
        {
            stream.CheckNotNull("stream");
            return Open(stream, Options.None);
        }

        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        public static SevenZipArchive Open(Stream stream, Options options)
        {
            stream.CheckNotNull("stream");
            return new SevenZipArchive(stream, options);
        }


        internal SevenZipArchive(Stream stream, Options options)
            : base(ArchiveType.SevenZip, stream.AsEnumerable(), options)
        {
        }

        internal SevenZipArchive()
            : base(ArchiveType.SevenZip)
        {
        }

        protected override IEnumerable<SevenZipVolume> LoadVolumes(IEnumerable<Stream> streams, Options options)
        {
            foreach (Stream s in streams)
            {
                if (!s.CanRead || !s.CanSeek)
                {
                    throw new ArgumentException("Stream is not readable and seekable");
                }
                SevenZipVolume volume = new SevenZipVolume(s, options);
                yield return volume;
            }
        }

        protected override IEnumerable<SevenZipArchiveEntry> LoadEntries(IEnumerable<SevenZipVolume> volumes)
        {
            var stream = volumes.Single().Stream;
            LoadFactory(stream);
            for (int i = 0; i < factory.Entries.Length; i++)
            {
                var file = factory.Entries[i];
                yield return new SevenZipArchiveEntry(this, new SevenZipFilePart(factory, i, file, stream));
            }
        }

        private void LoadFactory(Stream stream)
        {
            if (factory == null)
            {
                stream.Position = 0;
                factory = new SevenZipHeaderFactory(stream);
            }
        }


        public static bool IsSevenZipFile(Stream stream)
        {
            try
            {
                return SevenZipHeaderFactory.SignatureMatch(stream);
            }
            catch
            {
                return false;
            }
        }

        protected override IReader CreateReaderForSolidExtraction()
        {
            return new SevenZipReader(this);
        }

        public override bool IsSolid
        {
            get
            {
                return Entries.Where(x => !x.IsDirectory).GroupBy(x => x.FilePart.Header.Folder).Count() > 1;
            }
        }

        private class SevenZipReader : AbstractReader<SevenZipEntry, SevenZipVolume>
        {
            private readonly SevenZipArchive archive;
            private Folder currentFolder;
            private Stream currentStream;

            internal SevenZipReader(SevenZipArchive archive)
                : base(Options.KeepStreamsOpen, ArchiveType.SevenZip)
            {
                this.archive = archive;
            }


            public override SevenZipVolume Volume
            {
                get { return archive.Volumes.Single(); }
            }

            internal override IEnumerable<SevenZipEntry> GetEntries(Stream stream)
            {
                List<SevenZipArchiveEntry> entries = archive.Entries.ToList();
                stream.Position = 0;
                foreach (var dir in entries.Where(x => x.IsDirectory))
                {
                    yield return dir;
                }
                foreach (var group in entries.Where(x => !x.IsDirectory).GroupBy(x => x.FilePart.Header.Folder))
                {
                    currentFolder = group.Key;
                    currentStream = currentFolder.GetStream();
                    foreach (var entry in group.OrderBy(x => x.FilePart.Header.FolderOffset))
                    {
                        if (currentStream.Position != (long)entry.FilePart.Header.FolderOffset)
                        {
                            throw new InvalidFormatException("Unexpected SevenZip folder offset");
                        }
                        yield return entry;
                    }
                }
            }

            protected override EntryStream GetEntryStream()
            {
                return new EntryStream(new ReadOnlySubStream(currentStream,
                                             (long)
                                             Entry.Parts.Cast<SevenZipFilePart>().Single().Header.UnpackedStream.UnpackedSize));
            }
        }
    }
}