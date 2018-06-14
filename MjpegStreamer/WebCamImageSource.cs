using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace MjpegStreamer
{
    public static class WebCamImageSource
    {
        public static Image Snapshot( string imagUrl )
        {
            WebRequest request = WebRequest.Create( imagUrl );
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            return new Bitmap( responseStream );
        }

        internal static MemoryStream Stream( Image image )
        {
            MemoryStream stream = new MemoryStream();

            stream.SetLength( 0 );
            image.Save( stream, ImageFormat.Jpeg );
            return stream;
        }
    }
}
