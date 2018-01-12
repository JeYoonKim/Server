using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    class RouteAttribute : System.Attribute
    {
        private ushort att_route;

        public RouteAttribute(ushort route)
        {
            att_route = route;
        }

        public ushort Route
        {
            get { return att_route; }
        }

    }
}
