using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using Client.Packet;


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

        private static void StartClient(int i)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // (2) 서버에 연결
            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7000);
            sock.Connect(ep);


            string sendData = "testSending" + i;
            byte[] byteData = Encoding.UTF8.GetBytes(sendData);
            ushort routeData = 1;
            ushort lengthData = (ushort)byteData.Length;

            IList<ArraySegment<byte>> receiverPacket = new List<ArraySegment<byte>>();

            // (3) 서버에 데이타 전송
            sock.Send(DataToPacket(lengthData, routeData, byteData));


            // (4) 서버에서 데이타 수신
            int n = sock.Receive(receiverPacket);

            //string data = Encoding.UTF8.GetString(receiverBuff, 0, n);
            //Console.WriteLine(data);

            sock.Close();


        }

        static void Main(string[] args)
        {
            for (int i = 0; i < 200; i++)
                StartClient(i);
        }
    }


}
