using System;
using System.Collections.Generic;
using System.Text;
using Server.Context.Attribute;

namespace Server.Context.Attribute
{
    class SocketAttribute : ManagedAttribute
    {
        public SocketAttribute() : base(ManagedType.Singleton) { }
    }
}