using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Server.Packet;
using Server.Attribute;
using System.Reflection;
using GGM.Application.Service;

namespace Server
{

    public class TCPService : IService
    {


        public class PlayController
        {

            [Route(0)]
            public void UseCard()
            {
                Console.WriteLine("Use Card");
            }

            [Route(1)]
            public void DrawCard()
            {
                Console.WriteLine("Draw Card");
            }
            
        }

        public class NotificationController
        {
            [Route(0)]
            public void BroadCastReceive()
            {
                Console.WriteLine("BroadCast");
            }
        }

        public class TCPDispatcher
        {
            public Dictionary<Socket, Dictionary<ushort, Action>> dicSocket = new Dictionary<Socket, Dictionary<ushort, Action>>();

            public void RegisterController(Socket socket, object Controller)
            {
                Dictionary<ushort, Action> dicController = new Dictionary<ushort, Action>();

                Type T = Controller.GetType();
                ConstructorInfo constructorInfo = T.GetConstructor(Type.EmptyTypes);
                object classObject = constructorInfo.Invoke(new object[] { });

                MethodInfo[] methodInfos = Controller.GetType().GetMethods();
                

                foreach (MethodInfo methodInfo in methodInfos)
                {
                    var routeAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();
                    if(routeAttribute!=null)
                    dicController.Add(routeAttribute.Route, () => { methodInfo.Invoke(classObject, null); });
                }

                
                dicSocket.Add(socket, dicController);
            }
        }

        public class TCPSocket
        {
            TCPDispatcher tcpDispatcher = new TCPDispatcher();

            public void taskWait()
            {
                Task.WaitAll(RunPlaySocket(), RunNoticeSocket());
            }
            private IList<ArraySegment<byte>> DataToPacket(ushort lengthData, ushort routeData, byte[] byteData)
            {
                Packet.Packet tcpPacket = new Packet.Packet(lengthData, routeData, byteData);

                IList<ArraySegment<byte>> packet = tcpPacket.ToByteSegements();

                return packet;

            }
            async Task RunNoticeSocket()
            {
                Socket noticeSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint notice_ep = new IPEndPoint(IPAddress.Any, 7001);
                noticeSock.Bind(notice_ep);
                noticeSock.Listen(200);

                tcpDispatcher.RegisterController(noticeSock, new NotificationController());

                while (true)
                {
                    Socket clientSock = await Task.Factory.FromAsync(noticeSock.BeginAccept, noticeSock.EndAccept, null);


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

                    byte[] bodyBuff = new byte[bodyLength];
                    int bodyCount = await Task.Factory.FromAsync<int>(
                               clientSock.BeginReceive(bodyBuff, 0, bodyBuff.Length, SocketFlags.None, null, clientSock),
                               clientSock.EndReceive);

                    if (tcpDispatcher.dicSocket.ContainsKey(noticeSock).Equals(true))
                    {
                        Console.WriteLine("noitceworking");
                        if (tcpDispatcher.dicSocket[noticeSock].ContainsKey(route).Equals(true))
                        {

                            tcpDispatcher.dicSocket[noticeSock][route]();

                        }
                    }

                    if (bodyCount > 0)
                    {
                        string msg = Encoding.ASCII.GetString(bodyBuff, 0, bodyCount);
                        Console.WriteLine(msg);


                        await Task.Factory.FromAsync(
                                clientSock.BeginSend(DataToPacket((ushort)bodyLength, (ushort)routeCount, bodyBuff), SocketFlags.None, null, clientSock),
                                clientSock.EndSend);

                    }

                    clientSock.Close();
                }

            }
            async Task RunPlaySocket()
            {

                int MAX_SIZE = 1024;
                int i = 0;

                Socket playSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint play_ep = new IPEndPoint(IPAddress.Any, 7000);
                playSock.Bind(play_ep);
                playSock.Listen(200);

                tcpDispatcher.RegisterController(playSock, new PlayController());

                while (true)
                {
                    Socket clientSock = await Task.Factory.FromAsync(playSock.BeginAccept, playSock.EndAccept, null);


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

                    byte[] bodyBuff = new byte[bodyLength];
                    int bodyCount = await Task.Factory.FromAsync<int>(
                               clientSock.BeginReceive(bodyBuff, 0, bodyBuff.Length, SocketFlags.None, null, clientSock),
                               clientSock.EndReceive);

                    if (tcpDispatcher.dicSocket.ContainsKey(playSock).Equals(true))
                    {
                        Console.WriteLine("playworking");
                        if (tcpDispatcher.dicSocket[playSock].ContainsKey(route).Equals(true))
                        {

                            tcpDispatcher.dicSocket[playSock][route]();

                        }
                    }


                    if (bodyCount > 0)
                    {
                        string msg = Encoding.ASCII.GetString(bodyBuff, 0, bodyCount);
                        Console.WriteLine(msg);


                        await Task.Factory.FromAsync(
                                clientSock.BeginSend(DataToPacket((ushort)bodyLength, (ushort)routeCount, bodyBuff), SocketFlags.None, null, clientSock),
                                clientSock.EndSend);

                    }

                    clientSock.Close();
                }

            }
        }

        public Guid ID { get; set; }       

        public void Boot(string[] arguments)
        {
            //TODO: Start Server
            TCPSocket tcpSocket = new TCPSocket();
            tcpSocket.taskWait();
        }
      
    }
}