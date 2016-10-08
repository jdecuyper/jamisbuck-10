using System;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleSocketServer server = new SimpleSocketServer(7000);
            server.Start();
            Console.WriteLine("Press any key to close server");
            Console.ReadLine();
            server.Stop();
        }
    }
}