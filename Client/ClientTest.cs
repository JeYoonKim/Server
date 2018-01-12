using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Reflection;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using Client.Packet;
using Client.Attribute;

namespace Client
{
    #region State object define
    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 256;
        public List<ArraySegment<byte>> buffer;
        public StringBuilder sb = new StringBuilder();
    }
    #endregion

    class ClientTest
    {

      

        private static IList<ArraySegment<byte>> DataToPacket(ushort lengthData, ushort routeData, byte[] byteData)
        {
            Packet.Packet tcpPacket = new Packet.Packet(lengthData, routeData, byteData);

            IList<ArraySegment<byte>> packet = tcpPacket.ToByteSegements();

            return packet;

        }

        private static void StartClient()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7000);
            sock.Connect(ep);

            string sendData = "RouteZero";

           ushort route = 0;

            byte[] byteData = Encoding.UTF8.GetBytes(sendData);
            ushort lengthData = (ushort)byteData.Length;

            sock.Send(DataToPacket(lengthData, route, byteData));

            IList<ArraySegment<byte>> receiverPacket = new List<ArraySegment<byte>>();
            byte[] bodyLengthBuff = new byte[sizeof(ushort)];
            byte[] routeBuff = new byte[sizeof(ushort)];

            sock.Receive(bodyLengthBuff,SocketFlags.None);
            sock.Receive(routeBuff, SocketFlags.None);
            ushort bodyLength = BitConverter.ToUInt16(bodyLengthBuff, 0);
            byte[] bodyBuff = new byte[bodyLength];

            sock.Receive(bodyBuff, SocketFlags.None);


            //string data = Encoding.UTF8.GetString(receiverBuff, 0, n);
            //Console.WriteLine(data);

            sock.Close();


        }
        

        static void Main(string[] args)
        {
                StartClient();
        }
    }


}
