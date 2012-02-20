using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Mono.Cxxi
{
    public class ByRef<T>
    {
        public ByRef(IntPtr ptr)
        {
            this.Pointer = ptr;
        }

        public static implicit operator ByRef<T>(IntPtr ptr)
        {
            return new ByRef<T>(ptr);
        }

        public static implicit operator T(ByRef<T> byref)
        {
            return byref.Value;
        }

        public IntPtr Pointer
        {
            get;
            protected set;
        }

        public T Value
        {
            get
            {
                var s = new InternalStruct();
                Marshal.PtrToStructure(this.Pointer, s);
                return s.Value;
            }
            set
            {
                var s = new InternalStruct(value);
                Marshal.StructureToPtr(s, this.Pointer, false);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private class InternalStruct
        {
            public InternalStruct() : this(default(T)) { }
            public InternalStruct(T value)
            {
                this.Value = value;
            }

            public T Value;
        }
    }
}
