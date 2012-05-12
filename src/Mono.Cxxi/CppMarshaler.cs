using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Cxxi
{
    public static class CppMarshaler
    {
        public static T MarshalNativeToManaged<T>(IntPtr pNativeData, Func<T> creator)
            where T : class
        {
            if (pNativeData == IntPtr.Zero)
                return null;

            var instance = CppInstancePtr.ToManaged<T>(pNativeData);
            return instance ?? creator();
        }
    }
}
