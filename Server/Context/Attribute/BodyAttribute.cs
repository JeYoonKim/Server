using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Context.Attribute
{
    class BodyAttribute : ManagedAttribute
    {
        public BodyAttribute() : base(ManagedType.Singleton) { }
    }
}