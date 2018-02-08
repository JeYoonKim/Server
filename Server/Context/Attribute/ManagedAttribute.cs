using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Context.Attribute
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ManagedAttribute : System.Attribute
    {
        public ManagedAttribute(ManagedType managedType)
        {
            ManagedType = managedType;
        }

        /// <summary>
        ///     생명주기 타입을 지정합니다.
        /// </summary>
        public ManagedType ManagedType { get; private set; }
    }
}