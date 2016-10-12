using System;
using System.Net.Sockets;
using System.Text;

namespace SocketClient
{
    public class Client : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _response;

        public Client(string host, int port)
        {
            try
            {
                _client = new TcpClient(host, port);
            }
            catch (SocketException se)
            {
                Console.WriteLine(String.Format("Could not connect to host {0} on port {1}", host, port));
                Console.Write(se.ToString() + "\n\n");
            }

            if (_client != null)
                _stream = _client.GetStream();
        }

        public string ServerResponse
        {
            get { return _response; }
        }

        public bool IsReady
        {
            get { return _client != null; }
        }

        /// <summary>
        /// Send integer to server and receive its binary string representation
        /// as response.
        ///
        /// Not for self on how to convert a 32 bits integer into a 4-byte word:
        /// Use BitConverter.GetBytes to convert an integer 256 into a 4-byte array.
        ///
        /// Example 1:
        /// The integer 256 is converted into following byte array: [0,1,0,0] which is the
        /// same as [0000000, 10000000, 0000000, 0000000]
        /// If we reverse the array then we get the binary string representation
        /// of 256: 100000000
        ///
        /// Example 2:
        /// The integer 34 becomes: [34,0,0,0] => [100010, 0000000, 0000000, 0000000]
        /// Reverse the array and we get: 100010
        /// </summary>
        /// <param name="number">Integer that will sent to the server</param>
        public void Query(int number)
        {
            // Receive 6 bytes from server
            byte[] bytes = new byte[6];
            int bytesRead = _stream.Read(bytes, 0, 6);
            string ready = Encoding.ASCII.GetString(bytes, 0, 6);

            if (ready != "ready\n")
                return;

            // Send length of the query as a 4 byte-word
            string query = number.ToString();
            int length = query.Length;
            byte[] lengthBytes = BitConverter.GetBytes(length);
            _stream.Write(lengthBytes, 0, 4);

            // Send query
            var queryBytes = Encoding.ASCII.GetBytes(query);
            _stream.Write(queryBytes, 0, queryBytes.Length);

            // Receive length of the response as a 4 byte-word
            byte[] buffer = new byte[4];
            _stream.Read(buffer, 0, 4);
            int respLength = BitConverter.ToInt32(buffer, 0);
            Console.WriteLine("Lengh of response: {0}", respLength);

            // Receive response
            byte[] responseBytes = new byte[respLength];
            _stream.Read(responseBytes, 0, respLength);
            _response = Encoding.ASCII.GetString(responseBytes, 0, respLength);
        }

        public void Dispose()
        {
            if (IsReady)
                _client.Close();
        }
    }
}
