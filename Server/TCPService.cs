using System;
using GGM.Application.Service;

namespace Server
{
    public class TCPService : IService
    {
        public Guid ID { get; set; }

        public void Boot(string[] arguments)
        {
            //TODO: Start Server
        }
    }
}
