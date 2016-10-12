using System;

namespace SocketClient
{
    class Program
    {

        static void Main(string[] args)
        {
            int port = 7000;
            string host = "127.0.0.1";

            Console.WriteLine("usage: SocketClient [host] [port]\n");

            if (args.Length == 2)
            {
                if (!String.IsNullOrEmpty(args[0]))
                    host = args[0];

                int portArg = 0;
                if (int.TryParse(args[1], out portArg))
                    port = portArg;
            }

            using (Client c = new Client(host, port))
            {
                if (c.IsReady)
                {
                    Random r = new Random();
                    int number = r.Next();
                    Console.WriteLine(String.Format("Convert {0} into a binary string", number));
                    c.Query(number);
                    Console.WriteLine(String.Format("Binary string: {0}", c.ServerResponse));
                    int numberFromBinaryStr = Convert.ToInt32(c.ServerResponse, 2);
                    Console.WriteLine(String.Format("Convert binary string back to integer: {0}", numberFromBinaryStr));
                }
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}