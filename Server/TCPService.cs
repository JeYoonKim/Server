using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Server.Packet;

using GGM.Application.Service;

namespace Server
{
    public class TCPService : IService
    {
        public Guid ID { get; set; }

        public void Boot(string[] arguments)
        {
            //TODO: Start Server
            RunAsyncSocketServer().Wait();

        }

        async Task RunAsyncSocketServer()
        {
            int MAX_SIZE = 1024;

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 7000);
            sock.Bind(ep);
            sock.Listen(200);

            while (true)
            {
                Socket clientSock = await Task.Factory.FromAsync(sock.BeginAccept, sock.EndAccept, null);
                IList<ArraySegment<byte>> receiverPacket = new List<ArraySegment<byte>>();
                byte[] bodyLengthBuff = new byte[sizeof(ushort)];
                byte[] routeBuff = new byte[sizeof(ushort)];

                int bodyLengthCount = await Task.Factory.FromAsync<int>(
                           clientSock.BeginReceive(bodyLengthBuff, 0, bodyLengthBuff.Length, SocketFlags.None, null, clientSock),
                           clientSock.EndReceive);
                //Console.WriteLine(bodyLengthCount);
                ushort bodyLength = BitConverter.ToUInt16(bodyLengthBuff, 0);
                //Console.WriteLine(bodyLength);


                int routeCount = await Task.Factory.FromAsync<int>(
                           clientSock.BeginReceive(routeBuff, 0, routeBuff.Length, SocketFlags.None, null, clientSock),
                           clientSock.EndReceive);
                //Console.WriteLine(routeCount);
                ushort route = BitConverter.ToUInt16(routeBuff, 0);
                //Console.WriteLine(route);


                byte[] bodyBuff = new byte[bodyLength];
                int bodyCount = await Task.Factory.FromAsync<int>(
                           clientSock.BeginReceive(bodyBuff, 0, bodyBuff.Length, SocketFlags.None, null, clientSock),
                           clientSock.EndReceive);

                Packet.Packet packet = new Packet.Packet((ushort) bodyLength, (ushort) routeCount, bodyBuff);
                IList<ArraySegment<byte>> sendPacket = packet.ToByteSegements();

                if (bodyCount > 0)
                {
                    string msg = Encoding.ASCII.GetString(bodyBuff, 0, bodyCount);
                    Console.WriteLine(msg);
                    
                    await Task.Factory.FromAsync(
                            clientSock.BeginSend(sendPacket, SocketFlags.None, null, clientSock),
                            clientSock.EndSend);
                }

                clientSock.Close();
            }
        }
    }
}
