using System.IO;

namespace SharpCompress.Common.SevenZip
{
    public class SevenZipVolume : GenericVolume
    {
        public SevenZipVolume(Stream stream, Options options)
            : base(stream, options)
        {
        }


    }
}
