using System;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 6000;

            Console.WriteLine("usage: SocketServer [port]\n");

            if (args.Length == 1)
            {
                int portArg = 0;
                if (int.TryParse(args[0], out portArg))
                    port = portArg;

            }

            using (SimpleSocketServer server = new SimpleSocketServer(port))
            {
                server.Start();
                Console.ReadLine();
            }

        }
    }
}