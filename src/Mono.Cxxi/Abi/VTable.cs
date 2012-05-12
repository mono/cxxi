//
// Mono.Cxxi.Abi.VTable.cs: Managed VTable Implementation
//
// Author:
//   Alexander Corrado (alexander.corrado@gmail.com)
//   Andreia Gaita (shana@spoiledcat.net)
//
// Copyright (C) 2010-2011 Alexander Corrado
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Linq;

namespace Mono.Cxxi.Abi {

	// TODO: RTTI .. support virtual inheritance

	public class VTable : IDisposable {

		protected bool initialized;
		protected CppTypeInfo type_info;
		protected IntPtr vtPtr;

		public virtual int EntryCount {
			get { return type_info.VirtualMethods.Count; }
		}
		public virtual int EntrySize {
			get { return IntPtr.Size; }
		}

		// Subclasses should allocate vtPtr and then call WriteOverrides
		public VTable (CppTypeInfo typeInfo)
		{
			this.initialized = false;
			this.type_info = typeInfo;
			this.vtPtr = Marshal.AllocHGlobal ((EntryCount * EntrySize) + typeInfo.VTableTopPadding + typeInfo.VTableBottomPadding);

			WriteOverrides ();
			CppInstancePtr.RegisterManagedVTable (this);
		}

		protected virtual void WriteOverrides ()
		{
			IntPtr vtEntryPtr;
			int currentOffset = type_info.VTableTopPadding;
			for (int i = 0; i < EntryCount; i++) {
				Delegate currentOverride = type_info.VTableOverrides [i];

				if (currentOverride != null) // managed override
					vtEntryPtr = Marshal.GetFunctionPointerForDelegate (currentOverride);
				else
					vtEntryPtr = IntPtr.Zero;

				Marshal.WriteIntPtr (vtPtr, currentOffset, vtEntryPtr);
				currentOffset += EntrySize;
			}
		}

		public virtual T GetVirtualCallDelegate<T> (CppInstancePtr instance, int index)
			where T : class /*Delegate*/
		{
            var vtable = instance.GetNativeBaseVTable(this.TypeInfo.WrapperType);
            if (vtable == IntPtr.Zero)
                vtable = instance.NativeVTable;

			var ftnptr = Marshal.ReadIntPtr (vtable, (index * EntrySize));
			if (ftnptr == IntPtr.Zero)
				throw new NullReferenceException ("Native VTable contains null...possible abstract class???");

			var del = Marshal.GetDelegateForFunctionPointer (ftnptr, typeof (T));
			return del as T;
		}

        public virtual void InitInstance(ref CppInstancePtr instance)
        {
            InitInstanceOffset(ref instance, true, 0);
        }

		// FIXME: Make this method unsafe.. it would probably be much faster
		public virtual void InitInstanceOffset(ref CppInstancePtr instance, bool isPrimary, int vtableOffset)
		{
            if (this.TypeInfo.HasVTable)
            {
                var basePtr = Marshal.ReadIntPtr(instance.Native, vtableOffset);
                Debug.Assert(basePtr != IntPtr.Zero);

                var realPtr = this.Pointer;
                if (basePtr == realPtr)
                    return;

                // If this is the primary class or if this is not a managed alloc
                // then update the main vtable 
                // This basically means derived classes and non primary base classes
                if (isPrimary || !instance.IsManagedAlloc)
                    instance.NativeVTable = basePtr;
                else
                    instance.SetNativeBaseVTable(this.TypeInfo.WrapperType, basePtr);

                if (!initialized)
                {
                    // FIXME: This could probably be a more efficient memcpy
					int top = type_info.VTableTopPadding;
                    for (int i = 0; i < top; i++)
                        Marshal.WriteByte(vtPtr, i, Marshal.ReadByte(basePtr, i - top));

                    int currentOffset = 0;
                    for (int i = 0; i < EntryCount; i++)
                    {
                        if (Marshal.ReadIntPtr(realPtr, currentOffset) == IntPtr.Zero)
                            Marshal.WriteIntPtr(realPtr, currentOffset, Marshal.ReadIntPtr(basePtr, currentOffset));

                        currentOffset += EntrySize;
                    }

                    // FIXME: This could probably be a more efficient memcpy
                    for (int i = 0; i < type_info.VTableBottomPadding; i++)
                        Marshal.WriteByte(realPtr, currentOffset + i, Marshal.ReadByte(basePtr, currentOffset + i));

                    initialized = true;
                }

                Marshal.WriteIntPtr(instance.Native, vtableOffset, realPtr);
            }

            // TODO: I should probably be looking up the offsets via the vbtable rather than this
            if (isPrimary)
            {
                int offset = this.TypeInfo.NativeSize;
                foreach (var vbase in this.TypeInfo.GetVirtualBasesDistinct().Reverse())
                {
                    offset -= vbase.NativeSize;
                    vbase.VTable.InitInstanceOffset(ref instance, false, offset);
             
                    // Put a pointer to this virtual base in our instance so we can find it
                    instance.SetNativeBasePointer(vbase.WrapperType, new IntPtr(instance.Native.ToInt64() + offset));
                }
            }
		}

		public virtual void ResetInstance (CppInstancePtr instance)
		{
            // TODO: Should we reset the virtual base vftables as well?
            if (this.TypeInfo.HasVTable)
			    Marshal.WriteIntPtr (instance.Native, instance.NativeVTable);
		}

		public CppTypeInfo TypeInfo {
			get { return type_info; }
		}

		public IntPtr Pointer {
            get { return new IntPtr(vtPtr.ToInt64() + type_info.VTableTopPadding); }
		}

		protected virtual void Dispose (bool disposing)
		{
			if (vtPtr != IntPtr.Zero) {
				Marshal.FreeHGlobal (vtPtr);
				vtPtr = IntPtr.Zero;
			}
		}

		// TODO: This WON'T usually be called because VTables are associated with classes
		//  (not instances) and managed C++ class wrappers are staticly held?
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		~VTable ()
		{
			Dispose (false);
		}

		public static bool BindToSignatureAndAttribute (MemberInfo member, object obj)
		{
			var overrideNative = member.GetCustomAttributes (typeof (OverrideNativeAttribute), true);
			if (overrideNative.Length == 0)
				return false;

			var name = ((OverrideNativeAttribute)overrideNative [0]).NativeMethod ?? member.Name;

			return BindToSignature (member, obj, name);
		}

		public static bool BindToSignature (MemberInfo member, object obj)
		{
			return BindToSignature (member, obj, member.Name);
		}

		public static bool BindToSignature (MemberInfo member, object obj, string nativeMethod)
		{
			MethodInfo imethod = (MethodInfo) obj;
			MethodInfo candidate = (MethodInfo) member;

			if (nativeMethod != imethod.Name)
				return false;

			ParameterInfo[] invokeParams = imethod.GetParameters ();
			ParameterInfo[] methodParams = candidate.GetParameters ();

			if (invokeParams.Length == methodParams.Length) {
				for (int i = 0; i < invokeParams.Length; i++) {
					if (!invokeParams [i].ParameterType.IsAssignableFrom (methodParams [i].ParameterType))
						return false;
				}
			} else if (invokeParams.Length == methodParams.Length + 1) {
				for (int i = 1; i < invokeParams.Length; i++) {
					if (!invokeParams [i].ParameterType.IsAssignableFrom (methodParams [i - 1].ParameterType))
						return false;
				}
			} else
				return false;

			return true;
		}
	}
}
