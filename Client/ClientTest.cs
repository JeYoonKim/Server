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

        [Route(0)]
        private static void RouteZero(Socket sock)
        {
            string sendData = "RouteZero";

            MethodBase method = MethodBase.GetCurrentMethod();
            RouteAttribute attr = (RouteAttribute)method.GetCustomAttributes(typeof(RouteAttribute), true)[0];
            ushort route = attr.Route;

            byte[] byteData = Encoding.UTF8.GetBytes(sendData);
            ushort lengthData = (ushort)byteData.Length;

            sock.Send(DataToPacket(lengthData, route, byteData));

        }

        [Route(1)]
        private static void RouteOne(Socket sock)
        {
            string sendData = "RouteOne";
            MethodBase method = MethodBase.GetCurrentMethod();
            RouteAttribute attr = (RouteAttribute)method.GetCustomAttributes(typeof(RouteAttribute), true)[0];
            ushort route = attr.Route;
            byte[] byteData = Encoding.UTF8.GetBytes(sendData);
            ushort lengthData = (ushort)byteData.Length;
            sock.Send(DataToPacket(lengthData, route, byteData));
        }

        [Route(2)]
        private static void RouteTwo(Socket sock)
        {
            string sendData = "RouteTwo";

            MethodBase method = MethodBase.GetCurrentMethod();
            RouteAttribute attr = (RouteAttribute)method.GetCustomAttributes(typeof(RouteAttribute), true)[0];
            ushort route = attr.Route;

            byte[] byteData = Encoding.UTF8.GetBytes(sendData);
            ushort lengthData = (ushort)byteData.Length;

            sock.Send(DataToPacket(lengthData, route, byteData));

        }

        private static IList<ArraySegment<byte>> DataToPacket(ushort lengthData, ushort routeData, byte[] byteData)
        {
            Packet.Packet tcpPacket = new Packet.Packet(lengthData, routeData, byteData);

            IList<ArraySegment<byte>> packet = tcpPacket.ToByteSegements();

            return packet;

        }

        private static void StartClient()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // (2) 서버에 연결
            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7000);
            sock.Connect(ep);


            //RouteZero(sock);
            //RouteOne(sock);
            RouteTwo(sock);


            IList<ArraySegment<byte>> receiverPacket = new List<ArraySegment<byte>>();
            int n = sock.Receive(receiverPacket);

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
