using System;
using GGM.Application;
using Server;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            GGMApplication.Run(typeof(Program), args, typeof(TCPService));
        }
    }
}
