using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    class RouteAttribute : System.Attribute
    {
        private ushort _route;

        public RouteAttribute(ushort route)
        {
            _route = route;
        }

        public ushort Route
        {
            get { return _route; }
        }

    }
}
