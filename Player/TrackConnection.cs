using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Player {

    public class RecieveStateObject {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder ();
    }
    public enum ConnectionType {
        speedStreamer,
        trackUpdateStreamer,

    }
    public class ConnectStateObject {
        public ConnectStateObject (Socket socket, ConnectionType type) {
            Socket = socket;
            ConnectionType = type;
        }
        public Socket Socket { get; private set; }
        public ConnectionType ConnectionType { get; private set; }

        internal string GetTypeId () {
            switch (ConnectionType) {
                case ConnectionType.speedStreamer:
                    return "SpeedStreamer";
                case ConnectionType.trackUpdateStreamer:
                    return "TackUpdateStreamer";
                default:
                    return "";
            }
        }
    };
    public class TrackConnection : IDisposable {
        private char unitSeperatorChar = (char) Convert.ToInt32 ("0x1f", 16);

        private ManualResetEvent connectToSpeedSocketDone = new ManualResetEvent (false);
        private string response = String.Empty;
        private Socket _speedUpdateSocket;
        private Socket _trackUpdatesSocket;

        public TrackConnection () {
            _trackUpdatesSocket = CreateTrackSocket (ConnectionType.trackUpdateStreamer);
            _speedUpdateSocket = CreateTrackSocket (ConnectionType.speedStreamer);

            connectToSpeedSocketDone.WaitOne ();
        }

        private Socket CreateTrackSocket (ConnectionType type) {
            var hostname = Environment.GetEnvironmentVariable("RACE_TRACK_HOSTNAME");
            if (hostname == null)
            {
                hostname = "localhost";
            }

            var portString = Environment.GetEnvironmentVariable("RACE_TRACK_PORT");
            var port = 11000;
            if (portString != null)
            {
                port = Int32.Parse(portString);
            }
            
            
            try {
                IPHostEntry ipHostInfo = Dns.GetHostEntry (hostname);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                Socket client = new Socket (ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                var state = new ConnectStateObject (client, type);
                client.BeginConnect (ipAddress, port, new AsyncCallback (ConnectCallback), state);
                return client;

            } catch (Exception e) {
                Console.WriteLine (e.ToString ());
                throw;
            }
        }

        private void ConnectCallback (IAsyncResult ar) {
            var state = (ConnectStateObject) ar.AsyncState;
            try {

                state.Socket.EndConnect (ar);
                Console.WriteLine ("Socket connected to {0}", state.Socket.RemoteEndPoint.ToString ());

                state.Socket.Send (Encoding.ASCII.GetBytes (state.GetTypeId ()+ unitSeperatorChar));
                if (state.ConnectionType == ConnectionType.speedStreamer) {
                    connectToSpeedSocketDone.Set ();
                } else if (state.ConnectionType == ConnectionType.trackUpdateStreamer) {
                    ReceiveTrackUpdates (state.Socket);
                }
            } catch (Exception e) {
                CloseSocket (state.Socket);
                Console.WriteLine (e.ToString ());
                throw;
            }
        }

        public void ReceiveTrackUpdates (Socket socket) {
            try {
                RecieveStateObject state = new RecieveStateObject ();
                state.workSocket = socket;

                socket.BeginReceive (state.buffer, 0, RecieveStateObject.BufferSize, 0,
                    new AsyncCallback (ReceiveTrackUpdate), state);
            } catch (Exception e) {
                CloseSocket (socket);
                Console.WriteLine (e.ToString ());
            }
        }

        public string GetLatestResponse () {
            return response;
        }

        private void ReceiveTrackUpdate (IAsyncResult ar) {
            try {
                RecieveStateObject state = (RecieveStateObject) ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive (ar);

                if (bytesRead > 0) {
                    string value = Encoding.ASCII.GetString (state.buffer, 0, bytesRead);
                    state.sb.Append (value);
                    Console.WriteLine (value);
                }
                client.BeginReceive (state.buffer, 0, RecieveStateObject.BufferSize, 0,
                    new AsyncCallback (ReceiveTrackUpdate), state);
            } catch (Exception e) {
                CloseSocket ((Socket) ar.AsyncState);
                Console.WriteLine (e.ToString ());
            }
        }
        private void Send (Socket socket, string data) {
            try {
                byte[] byteData = Encoding.ASCII.GetBytes (data + unitSeperatorChar);
                socket.BeginSend (byteData, 0, byteData.Length, 0,
                    new AsyncCallback (SendCallback), socket);

            } catch (System.Exception e) {
                CloseSocket (socket);
                Console.WriteLine (e.ToString ());
                throw;
            }
        }

        public void SendSpeed (int speed) {
            Send (_speedUpdateSocket, speed.ToString ());
        }
        private void SendCallback (IAsyncResult ar) {
            try {
                Socket client = (Socket) ar.AsyncState;

                int bytesSent = client.EndSend (ar);
                Console.WriteLine ("Sent {0} bytes to server.", bytesSent);
            } catch (Exception e) {
                CloseSocket ((Socket) ar.AsyncState);
                Console.WriteLine (e.ToString ());
            }
        }
        public void Dispose () {
            CloseSocket (_speedUpdateSocket);
            CloseSocket (_trackUpdatesSocket);
        }

        private void CloseSocket (Socket socket) {
            if (socket == null) return;

            if (socket.Connected) {
                socket.Shutdown (SocketShutdown.Both);
            }
            socket.Close ();
        }
    }
}