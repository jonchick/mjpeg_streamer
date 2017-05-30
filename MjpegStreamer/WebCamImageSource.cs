using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MjpegStreamer
{
    public static class WebCamImageSource
    {
        public static Image Snapshot()
        {
            WebRequest request = WebRequest.Create( "http://vps.yolahome.ru/webcam" );
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
