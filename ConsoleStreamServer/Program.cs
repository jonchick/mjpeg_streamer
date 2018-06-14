using MjpegStreamer;
using System;

namespace ConsoleStreamServer
{
    class Program
    {
        static void Main( string[] args )
        {
            int port = 8888;
            string imageUrl = "http://yolahome.ru/webcam_yard2";

            var server = new ImageStreamingServer();
            server.Start( port, imageUrl );

            Console.WriteLine( $"http://{Environment.MachineName}:{port}" );
            Console.Write( "Press <Esc> to exit... " );
            while ( Console.ReadKey().Key != ConsoleKey.Escape ) { }
        }
    }
}
