using System;
using System.Collections.Generic;

namespace Client.Packet
{
    class Packet
    {
        public Packet(ushort bodyLength, ushort route, byte[] body)
        {
            Route = route;
            BodyLength = bodyLength;
            Body = body;
        }

        public const ushort HEADER_SIZE = 4;
        // headerLength
        // bodyLength
        // route
        //TODO: 나중에 자료형에 괸한 고민이 필요함.
        public ushort BodyLength { get; private set; }
        public ushort Route { get; private set; }
        public byte[] Body;

        public byte[] ToBytes()
        {
            var bytes = new byte[sizeof(ushort) + sizeof(ushort) + BodyLength];
            System.Buffer.BlockCopy(BitConverter.GetBytes(BodyLength), 0, bytes, 0, sizeof(ushort));
            System.Buffer.BlockCopy(BitConverter.GetBytes(Route), 0, bytes, 2, sizeof(ushort));
            System.Buffer.BlockCopy(Body, 0, bytes, 4, BodyLength);
            return bytes;
        }

        public IList<ArraySegment<byte>> ToByteSegements()
        {
            var bodyLengthBytes = new ArraySegment<byte>(BitConverter.GetBytes(BodyLength));
            var routeBytes = new ArraySegment<byte>(BitConverter.GetBytes(Route));
            return new List<ArraySegment<byte>> { bodyLengthBytes, routeBytes, new ArraySegment<byte>(Body) };
        }
    }

}

