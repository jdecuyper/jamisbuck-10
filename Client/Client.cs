using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient
{
    public class Client : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _response;
        private const int PORT_NO = 7000;
        private const string SERVER_IP = "127.0.0.1";

        public Client()
        {
            _client = new TcpClient(SERVER_IP, PORT_NO);
            _stream = _client.GetStream();
        }

        public Client(string host, int port)
        {
            _client = new TcpClient(host, port);
            _stream = _client.GetStream();
        }

        public string ServerResponse
        {
            get { return _response; }
        }

        /// <summary>
        /// Send number to server and receive as response a binary string.
        /// </summary>
        /// <param name="query">Number to convert to a binary string</param>
        public void Query(int number)
        {
            while (!_stream.DataAvailable)
            {
                // Waiting for data from server
                Thread.Sleep(5000);
            }

            // Read first response from server (should be ready\n)
            byte[] bytes = new byte[_client.ReceiveBufferSize];
            int bytesRead = _stream.Read(bytes, 0, _client.ReceiveBufferSize);
            string ready = Encoding.ASCII.GetString(bytes, 0, 6);

            if (ready != "ready\n")
                return;

            // Send query 
            // Send number
            bytes = ASCIIEncoding.ASCII.GetBytes(number.ToString());
            _stream.Write(bytes, 0, bytes.Length);

            // Receive response
            bytes = new byte[_client.ReceiveBufferSize];
            bytesRead = _stream.Read(bytes, 0, _client.ReceiveBufferSize);
            _response = Encoding.ASCII.GetString(bytes, 0, bytesRead);
        }

        public void Dispose()
        {
            _client.Close();
        }
    }
}
