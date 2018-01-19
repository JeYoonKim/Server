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
using System.Reflection.Emit;


namespace Server
{

    public class TCPService : IService
    {

        public delegate void MethodDelegate();

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

            public Dictionary<Socket, Dictionary<ushort, Action>> SocketToDictionaryMap = new Dictionary<Socket, Dictionary<ushort, Action>>();

            private MethodDelegate CreateMethodDelegate(MethodInfo methodInfo, Object controller)
            {
                DynamicMethod dm = new DynamicMethod(methodInfo.Name,
                            methodInfo.ReturnType,
                             new[] { controller.GetType() },
                             GetType());

                ILGenerator iLGenerator = dm.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Call, methodInfo);
                iLGenerator.Emit(OpCodes.Ret);

                MethodDelegate methodDelegate = (MethodDelegate)dm.CreateDelegate(typeof(MethodDelegate));

                return methodDelegate;
            }


            public void RegisterController(Socket socket, object controller)
            {
                Dictionary<ushort, Action> _routeToActionMap = new Dictionary<ushort, Action>();

                MethodInfo[] methodInfos = controller.GetType().GetMethods();


                foreach (MethodInfo methodInfo in methodInfos)
                {
                    var routeAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();

                    if (routeAttribute != null)
                    {
                        var methodDelegate = CreateMethodDelegate(methodInfo, controller);
                        
                        _routeToActionMap.Add(routeAttribute.Route,() => methodDelegate());
                    }
                }

                SocketToDictionaryMap.Add(socket, _routeToActionMap);
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
                NotificationController notificationController = new NotificationController();
                tcpDispatcher.RegisterController(noticeSock, notificationController);

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

                    if (tcpDispatcher.SocketToDictionaryMap.ContainsKey(noticeSock).Equals(true))
                    {
                        //Console.WriteLine("noticeworking");
                        if (tcpDispatcher.SocketToDictionaryMap[noticeSock].ContainsKey(route).Equals(true))
                            tcpDispatcher.SocketToDictionaryMap[noticeSock][route]();
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
                PlayController playController = new PlayController();

                tcpDispatcher.RegisterController(playSock, playController);

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

                    if (tcpDispatcher.SocketToDictionaryMap.ContainsKey(playSock).Equals(true))
                    {
                        if (tcpDispatcher.SocketToDictionaryMap[playSock].ContainsKey(route).Equals(true))
                            tcpDispatcher.SocketToDictionaryMap[playSock][route]();
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