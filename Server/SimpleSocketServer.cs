using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    class SimpleSocketServer : IDisposable
    {
        private Thread _serverThread;
        private TcpListener _listener;
        private int _port;
        private bool _isListening = false;

        public int Port
        {
            get { return _port; }
            private set { }
        }

        /// <summary>
        /// Construct server with given server port.
        /// </summary>
        /// <param name="port">Port of the server.</param>
        public SimpleSocketServer(int port)
        {
            this.Initialize(port);
        }

        /// <summary>
        /// Start server's main thread.
        /// </summary>
        public void Start()
        {
            Console.WriteLine("Start server, press any key to shut down...");
            _serverThread.Start();
        }

        private void Stop()
        {
            Console.WriteLine("Stop server...");
            if (!_isListening)
                return;

            _serverThread.Abort();
            if (_listener != null)
                _listener.Stop();
        }

        private void Listen()
        {
            _listener = new TcpListener(IPAddress.Any, this._port);
            Console.WriteLine(String.Format("Listening on port {0}...", _port));

            try
            {
                _listener.Start();
                _isListening = true;
            }
            catch (SocketException se)
            {
                Console.WriteLine(String.Format("Could not create TCP listener on port {0}", _port));
                Console.Write(se.ToString() + "\n\n");
                _isListening = false;
            }

            while (true && _isListening)
            {
                if (!_listener.Pending())
                {
                    Thread.Sleep(1000);
                    continue; // skip to next iteration of loop
                }

                TcpClient client = _listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ThreadProc, client);
            }
        }

        private void ThreadProc(object obj)
        {
            var client = (TcpClient)obj;

            NetworkStream stream = client.GetStream();

            if (stream.CanRead && stream.CanWrite)
            {
                SendReady(stream);
                string query = ReadQuery(client, stream);
                SendResponse(client, stream, query);
            }

            Console.WriteLine("Close connection");
            client.Close();
        }

        private void SendReady(NetworkStream stream)
        {
            Console.WriteLine("Send 'ready' to client");
            var data = Encoding.ASCII.GetBytes("ready\n");
            stream.Write(data, 0, data.Length);
        }

        private string ReadQuery(TcpClient client, NetworkStream stream)
        {
            // Receive lenght of the query
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            int length = BitConverter.ToInt32(buffer, 0);
            Console.WriteLine("Read length of the query: {0}", length);

            // Receive query
            byte[] query = new byte[length];
            stream.Read(query, 0, length);
            string queryTxt = Encoding.ASCII.GetString(query, 0, length);
            Console.WriteLine("Read query: {0}", queryTxt);

            return queryTxt;
        }

        private void SendResponse(TcpClient client, NetworkStream stream, string query)
        {
            // Convert digit its binary representation
            string response = Convert.ToString(int.Parse(query), 2);

            // Send length of the response
            int length = response.Length;
            byte[] lengthBytes = BitConverter.GetBytes(length);
            stream.Write(lengthBytes, 0, 4);
            Console.WriteLine("Send length of the response: {0}", length);

            // Send response
            byte[] responseBytes = Encoding.ASCII.GetBytes(Convert.ToString(int.Parse(query), 2));
            stream.Write(responseBytes, 0, length);
            Console.WriteLine("Send response: {0}", response);
        }

        private void Initialize(int port)
        {
            this._port = port;
            _serverThread = new Thread(this.Listen);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
