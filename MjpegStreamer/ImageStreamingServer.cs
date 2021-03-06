﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MjpegStreamer
{

    /// <summary>
    /// Provides a streaming server that can be used to stream any images source
    /// to any client.
    /// </summary>
    public class ImageStreamingServer : IDisposable
    {

        private List<Socket> _Clients;
        private Thread _Thread;
        
        public ImageStreamingServer()
        {

            _Clients = new List<Socket>();
            _Thread = null;
            
            this.Interval = 2000;
        }

        /// <summary>
        /// Gets or sets the interval in milliseconds (or the delay time) between 
        /// the each image and the other of the stream (the default is . 
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Gets a collection of client sockets.
        /// </summary>
        public IEnumerable<Socket> Clients { get { return _Clients; } }

        /// <summary>
        /// Returns the status of the server. True means the server is currently 
        /// running and ready to serve any client requests.
        /// </summary>
        public bool IsRunning { get { return ( _Thread != null && _Thread.IsAlive ); } }

        private string ImageUrl { get; set; }

        /// <summary>
        /// Starts the server to accepts any new connections on the specified port.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="imageUrl"></param>
        public void Start( int port, string imageUrl )
        {
            ImageUrl = imageUrl;
            lock ( this )
            {
                _Thread = new Thread( new ParameterizedThreadStart( ServerThread ) );
                _Thread.IsBackground = true;
                _Thread.Start( port );
            }

        }

        public void Stop()
        {

            if ( this.IsRunning )
            {
                try
                {
                    _Thread.Join();
                    _Thread.Abort();
                }
                finally
                {
                    lock ( _Clients )
                    {
                        foreach ( var s in _Clients )
                        {
                            try
                            {
                                s.Close();
                            }
                            catch { }
                        }
                        _Clients.Clear();
                    }
                    _Thread = null;
                }
            }
        }

        /// <summary>
        /// This the main thread of the server that serves all the new 
        /// connections from clients.
        /// </summary>
        /// <param name="state"></param>
        private void ServerThread( object state )
        {
            try
            {
                Socket Server = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

                Server.Bind( new IPEndPoint( IPAddress.Any, (int)state ) );
                Server.Listen( 10 );

                System.Diagnostics.Debug.WriteLine( string.Format( "Server started on port {0}.", state ) );

                foreach ( Socket client in Server.IncommingConnectoins() )
                {
                    ThreadPool.QueueUserWorkItem( new WaitCallback( ClientThread ), client );
                }

            }
            catch { }

            this.Stop();
        }

        /// <summary>
        /// Each client connection will be served by this thread.
        /// </summary>
        /// <param name="client"></param>
        private void ClientThread( object client )
        {
            Socket socket = (Socket) client;

            System.Diagnostics.Debug.WriteLine( string.Format( "New client from {0}", socket.RemoteEndPoint.ToString() ) );

            lock ( _Clients )
            {
                _Clients.Add( socket );
            }

            try
            {
                using ( MjpegWriter wr = new MjpegWriter( new NetworkStream( socket, true ) ) )
                {
                    // Writes the response header to the client.
                    wr.WriteHeader();

                    // Streams the images from the source to the client.
                    while ( true ) { 
                        if ( this.Interval > 0 )
                        {
                            Thread.Sleep( this.Interval );
                        }
                        Image image = WebCamImageSource.Snapshot( ImageUrl );
                        MemoryStream imgStream = WebCamImageSource.Stream( image );

                        wr.Write( imgStream );

                        image.Dispose();
                        imgStream.Dispose();
                    }
                }
            }
            catch { }
            finally
            {
                lock ( _Clients )
                {
                    _Clients.Remove( socket );
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Stop();
        }

        #endregion
    }

    static class SocketExtensions
    {
        public static IEnumerable<Socket> IncommingConnectoins( this Socket server )
        {
            while ( true )
            {
                yield return server.Accept();
            }
        }

    }
}
