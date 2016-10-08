using System;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Client c = new Client()) {
                c.Query(300321);
                Console.WriteLine(c.ServerResponse);
            }

            Console.ReadLine();
        }
    }
}