using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    class SimpleSocketServer
    {
        private Thread _serverThread;
        private TcpListener _listener;
        private int _port = 7000;
        private bool _isRunning = false;

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
            Console.WriteLine("Start server...");
            _isRunning = true;
            _serverThread.Start();
        }

        public void Stop()
        {
            Console.WriteLine("Stop server...");
            if (!_isRunning)
                return;

            _serverThread.Abort();
            if (_listener != null)
                _listener.Stop();
        }

        private void Listen()
        {
            _listener = new TcpListener(IPAddress.Any, this._port);
            Console.WriteLine("Listening...");
            _listener.Start();

            while (true)
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

            // Send ready\n
            string dataToSend = "ready\n";
            Console.WriteLine("Send: " + dataToSend);
            var data = Encoding.ASCII.GetBytes(dataToSend);
            stream.Write(data, 0, data.Length);

            // Wait for client to respond
            while (!stream.DataAvailable)
            {
                Console.WriteLine("Waiting for data...");
                Thread.Sleep(1000);
            }

            // Receive query from client
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
            string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Receive query: " + dataReceived);

            // Send response to client
            Console.WriteLine("Send the response");
            dataToSend = Convert.ToString(int.Parse(dataReceived), 2);
            data = Encoding.ASCII.GetBytes(dataToSend);
            stream.Write(data, 0, data.Length);
            stream.Flush();

            Console.WriteLine("Close connection");
            client.Close();
        }

        private void Initialize(int port)
        {
            this._port = port;
            _serverThread = new Thread(this.Listen);
        }
    }
}
