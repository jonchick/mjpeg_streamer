using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MjpegStreamer
{
    public static class WebCamImageSource
    {
        public static IEnumerable<Image> Snapshots()
        {
            Bitmap bitmap;
            while ( true )
            {
                WebRequest request = WebRequest.Create( "http://vps.yolahome.ru/webcam" );
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                bitmap = new Bitmap( responseStream );

                yield return bitmap;
            }

            // bitmap.Dispose();
            // yield break;
        }

        internal static IEnumerable<MemoryStream> Streams( this IEnumerable<Image> source )
        {
            MemoryStream ms = new MemoryStream();

            foreach ( var img in source )
            {
                ms.SetLength( 0 );
                img.Save( ms, System.Drawing.Imaging.ImageFormat.Jpeg );
                yield return ms;
            }

            ms.Close();
            ms = null;

            yield break;
        }
    }
}
