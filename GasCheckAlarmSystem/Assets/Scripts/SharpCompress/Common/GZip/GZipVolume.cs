using System.IO;

namespace SharpCompress.Common.GZip
{
    public class GZipVolume : Volume
    {


        public GZipVolume(Stream stream, Options options)
            : base(stream, options)
        {
        }




        public override bool IsFirstVolume
        {
            get
            {
                return true;
            }
        }

        public override bool IsMultiVolume
        {
            get
            {
                return true;
            }
        }
    }
}
