using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Server.Packet;
using Server.Context.Attribute;
using System.Reflection;
using GGM.Application.Service;
using System.Reflection.Emit;


namespace Server
{

    public class TCPService : IService
    {

        public delegate void RouteZeroDelegate();
        public delegate void RouteOneDelegate(byte[] bytes);
        public delegate void RouteTwoDelegate(LoginRequest loginRequest);
        public delegate void RouteThreeDelegate(Socket socket);
        public delegate void RouteFourDelegate(Socket socket, LoginRequest loginRequest);
        public delegate void RouteFiveDelegate(Socket socket);
        public delegate void RouteSixDelegate(Socket socket);




        public class LoginRequest
        {

        }

        public class PlayController
        {
            
            [Route(0)]
            public void EmptyRoute()
            {
                Console.WriteLine("route 0 working");
            }
            
            [Route(1)]
            public void ByteBodyRoute([Body] byte[] bytes)
            {
                string msg = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                Console.WriteLine(msg);
                Console.WriteLine("route 1 working");

            }

            [Route(2)]
            public void ObjectBodyRoute([Body] LoginRequest loginRequest)
            {
                Console.WriteLine("route 2 working");

            } 

            [Route(3)]
            public void SocketRoute([Socket] Socket socket)
            {
                Console.WriteLine("route 3 working");

            } 

            [Route(4)]
            public void Login([Socket] Socket socket, [Body] LoginRequest loginRequest)
            {
                Console.WriteLine("route 4 working");

            }

            [Route(5)]
            public void DeclearDropOut([Socket] Socket socket)
            {
                Console.WriteLine("route 5 working");

            }
        }

        public class NotificationController
        {
             [Route(0)]
            public void EmptyRoute()
            {
                Console.WriteLine("route 0 working");
            }
            
            [Route(1)]
            public void ByteBodyRoute([Body] byte[] bytes)
            {
                string msg = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                Console.WriteLine(msg);
                Console.WriteLine("route 1 working");

            }

            [Route(2)]
            public void ObjectBodyRoute([Body] LoginRequest loginRequest)
            {
                Console.WriteLine("route 2 working");

            } 

            [Route(3)]
            public void SocketRoute([Socket] Socket socket)
            {
                Console.WriteLine("route 3 working");

            } 

            [Route(5)]
            public void DeclearDropOut([Socket] Socket socket)
            {
                Console.WriteLine("route 5 working");

            }

            [Route(6)]
            public void Notice([Socket] Socket socket)
            {
                Console.WriteLine("route 6 working");

            }
        }

       

        public class TCPDispatcher
        {
            public Dictionary<Socket, Dictionary<ushort, Action>> SocketToDictionaryMap = new Dictionary<Socket, Dictionary<ushort, Action>>();

            private byte[] _bytes;
            private Socket _socket;


            public void init(byte[] bytes, Socket socket)
            {
                _bytes = bytes;
                _socket = socket;

            }

            private RouteZeroDelegate CreateRouteZeroDelegate(MethodInfo methodInfo, Object controller)
            {
                DynamicMethod dm = new DynamicMethod(methodInfo.Name,
                            methodInfo.ReturnType,
                             new[] { controller.GetType()},
                             GetType());

                ILGenerator iLGenerator = dm.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Call, methodInfo);
                iLGenerator.Emit(OpCodes.Ret);

                RouteZeroDelegate routeZeroDelegate = (RouteZeroDelegate)dm.CreateDelegate(typeof(RouteZeroDelegate));

                return routeZeroDelegate;
            }

            private RouteOneDelegate CreateRouteOneDelegate(MethodInfo methodInfo, Object controller)
            {
                DynamicMethod dm = new DynamicMethod(methodInfo.Name,
                            methodInfo.ReturnType,
                             new[] { controller.GetType(), typeof(byte[]) },
                             GetType());

                ILGenerator iLGenerator = dm.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Call, methodInfo);
                iLGenerator.Emit(OpCodes.Ret);

                RouteOneDelegate routeOneDelegate = (RouteOneDelegate)dm.CreateDelegate(typeof(RouteOneDelegate));

                return routeOneDelegate;
            }

            private RouteTwoDelegate CreateRouteTwoDelegate(MethodInfo methodInfo, Object controller)
            {
                DynamicMethod dm = new DynamicMethod(methodInfo.Name,
                            methodInfo.ReturnType,
                             new[] { controller.GetType(), typeof(LoginRequest) },
                             GetType());

                ILGenerator iLGenerator = dm.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Call, methodInfo);
                iLGenerator.Emit(OpCodes.Ret);

                RouteTwoDelegate routeTwoDelegate = (RouteTwoDelegate)dm.CreateDelegate(typeof(RouteTwoDelegate));

                return routeTwoDelegate;
            }

            private RouteThreeDelegate CreateRouteThreeDelegate(MethodInfo methodInfo, Object controller)
            {
                DynamicMethod dm = new DynamicMethod(methodInfo.Name,
                            methodInfo.ReturnType,
                             new[] { controller.GetType(), typeof(Socket) },
                             GetType());

                ILGenerator iLGenerator = dm.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Call, methodInfo);
                iLGenerator.Emit(OpCodes.Ret);

                RouteThreeDelegate routeThreeDelegate = (RouteThreeDelegate)dm.CreateDelegate(typeof(RouteThreeDelegate));

                return routeThreeDelegate;
            }

            private RouteFourDelegate CreateRouteFourDelegate(MethodInfo methodInfo, Object controller)
            {
                DynamicMethod dm = new DynamicMethod(methodInfo.Name,
                            methodInfo.ReturnType,
                             new[] { controller.GetType(), typeof(Socket), typeof(LoginRequest) },
                             GetType());

                ILGenerator iLGenerator = dm.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Ldarg_2);
                iLGenerator.Emit(OpCodes.Call, methodInfo);
                iLGenerator.Emit(OpCodes.Ret);

                RouteFourDelegate routeFourDelegate = (RouteFourDelegate)dm.CreateDelegate(typeof(RouteFourDelegate));

                return routeFourDelegate;
            }

            private RouteFiveDelegate CreateRouteFiveDelegate(MethodInfo methodInfo, Object controller)
            {
                DynamicMethod dm = new DynamicMethod(methodInfo.Name,
                            methodInfo.ReturnType,
                             new[] { controller.GetType(), typeof(Socket) },
                             GetType());

                ILGenerator iLGenerator = dm.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Call, methodInfo);
                iLGenerator.Emit(OpCodes.Ret);

                RouteFiveDelegate routeFiveDelegate = (RouteFiveDelegate)dm.CreateDelegate(typeof(RouteFiveDelegate));

                return routeFiveDelegate;
            }

            private RouteSixDelegate CreateRouteSixDelegate(MethodInfo methodInfo, Object controller)
            {
                DynamicMethod dm = new DynamicMethod(methodInfo.Name,
                            methodInfo.ReturnType,
                             new[] { controller.GetType(), typeof(Socket) },
                             GetType());

                ILGenerator iLGenerator = dm.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Call, methodInfo);
                iLGenerator.Emit(OpCodes.Ret);

                RouteSixDelegate routeSixDelegate = (RouteSixDelegate)dm.CreateDelegate(typeof(RouteSixDelegate));

                return routeSixDelegate;
            }



            public void RegisterController(Socket socket, object controller)
            {
                Dictionary<ushort, Action> _routeToActionMap = new Dictionary<ushort, Action>();

                MethodInfo[] methodInfos = controller.GetType().GetMethods();
                LoginRequest loginRequest = new LoginRequest();


                foreach (MethodInfo methodInfo in methodInfos)
                {
                    
                    var routeAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();

                    if (routeAttribute != null)
                    {

                      
                        switch (routeAttribute.Route)
                        {
                            case 0:

                                var routeZeroDelegate = CreateRouteZeroDelegate(methodInfo, controller);
                                _routeToActionMap.Add(routeAttribute.Route, () => routeZeroDelegate());

                                break;

                            case 1:
                                var routeOneDelegate = CreateRouteOneDelegate(methodInfo, controller);
                                _routeToActionMap.Add(routeAttribute.Route, () => routeOneDelegate(_bytes));

                                break;
                            case 2:
                                var routeTwoDelegate = CreateRouteTwoDelegate(methodInfo, controller);
                                _routeToActionMap.Add(routeAttribute.Route, () => routeTwoDelegate(loginRequest));

                                break;

                            case 3:
                                var routeThreeDelegate = CreateRouteThreeDelegate(methodInfo, controller);
                                _routeToActionMap.Add(routeAttribute.Route, () => routeThreeDelegate(_socket));

                                break;

                            case 4:
                                var routeFourDelegate = CreateRouteFourDelegate(methodInfo, controller);
                                _routeToActionMap.Add(routeAttribute.Route, () => routeFourDelegate(_socket,loginRequest));
                                break;

                            case 5:
                                var routeFiveDelegate = CreateRouteFiveDelegate(methodInfo, controller);
                                _routeToActionMap.Add(routeAttribute.Route, () => routeFiveDelegate(_socket));
                                break;

                            case 6:
                                var routeSixDelegate = CreateRouteSixDelegate(methodInfo, controller);
                                _routeToActionMap.Add(routeAttribute.Route, () => routeSixDelegate(_socket));
                                break;

                            default:
                                Console.WriteLine("Error");
                                break;
                        }

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

                    Packet.Packet packet = new Packet.Packet(bodyLength, route, bodyBuff);

                    tcpDispatcher.init(bodyBuff, clientSock);

                    //tcpDispatcher.RegisterController(noticeSock, notificationController, packet);

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
                               clientSock.BeginReceive(bodyLengthBuff, 0, bodyLengthBuff.Length, SocketFlags.None, null, playSock),
                               clientSock.EndReceive);
                    //Console.WriteLine(bodyLengthCount);
                    ushort bodyLength = BitConverter.ToUInt16(bodyLengthBuff, 0);
                    //Console.WriteLine(bodyLength);


                    int routeCount = await Task.Factory.FromAsync<int>(
                               clientSock.BeginReceive(routeBuff, 0, routeBuff.Length, SocketFlags.None, null, playSock),
                               clientSock.EndReceive);
                    //Console.WriteLine(routeCount);
                    ushort route = BitConverter.ToUInt16(routeBuff, 0);

                    byte[] bodyBuff = new byte[bodyLength];
                    int bodyCount = await Task.Factory.FromAsync<int>(
                               clientSock.BeginReceive(bodyBuff, 0, bodyBuff.Length, SocketFlags.None, null, playSock),
                               clientSock.EndReceive);

                    Packet.Packet packet = new Packet.Packet(bodyLength, route, bodyBuff);

                    tcpDispatcher.init(bodyBuff, clientSock);

                    if (tcpDispatcher.SocketToDictionaryMap.ContainsKey(playSock).Equals(true))
                    {
                        //Console.WriteLine("working");

                        if (tcpDispatcher.SocketToDictionaryMap[playSock].ContainsKey(route).Equals(true))
                        {
                            //Console.WriteLine("working");
                            tcpDispatcher.SocketToDictionaryMap[playSock][route]();

                        }

                    }
                    if (bodyCount > 0)
                    {
                        string msg = Encoding.ASCII.GetString(bodyBuff, 0, bodyCount);
                        Console.WriteLine(msg);


                        await Task.Factory.FromAsync(
                                playSock.BeginSend(DataToPacket((ushort)bodyLength, (ushort)routeCount, bodyBuff), SocketFlags.None, null, playSock),
                                playSock.EndSend);

                    }

                    playSock.Close();
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