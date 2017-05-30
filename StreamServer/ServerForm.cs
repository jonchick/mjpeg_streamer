using System;
using System.Linq;
using System.Windows.Forms;
using MjpegStreamer;

namespace rtaVideoStreamer
{
    public partial class ServerForm : Form
    {

        private ImageStreamingServer _Server;

        public ServerForm()
        {
            InitializeComponent();
            this.linkLabel1.Text = string.Format( "http://{0}:8080", Environment.MachineName );
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            _Server = new ImageStreamingServer();
            _Server.Start( 8080 );
        }

        private DateTime time = DateTime.MinValue;

        private void timer1_Tick( object sender, EventArgs e )
        {
            int count = ( _Server.Clients != null ) ? _Server.Clients.Count() : 0;

            this.sts.Text = "Clients: " + count.ToString();
        }

        private void linkLabel1_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( "firefox", this.linkLabel1.Text );

        }
    }
}
