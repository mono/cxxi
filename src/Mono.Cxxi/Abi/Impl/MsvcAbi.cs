//
// Mono.Cxxi.Abi.MsvcAbi.cs: An implementation of the Microsoft Visual C++ ABI
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
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Mono.Cxxi;
using Mono.Cxxi.Util;

namespace Mono.Cxxi.Abi {

	// FIXME: No 64-bit support
	public class MsvcAbi : CppAbi {

		public static readonly MsvcAbi Instance = new MsvcAbi ();

		private MsvcAbi ()
		{
		}

		public override CallingConvention? GetCallingConvention (MethodInfo methodInfo)
		{
			// FIXME: Varargs methods ... ?

			if (IsStatic (methodInfo))
				return CallingConvention.Cdecl;
			else
				return CallingConvention.ThisCall;
		}

		protected override string GetMangledMethodName (CppTypeInfo typeInfo, MethodInfo methodInfo)
		{
			var methodName = methodInfo.Name;
			var type = typeInfo.GetMangleType ();
			var className = type.ElementTypeName;
            var backReferences = new BackReferenceList();

			MethodType methodType = GetMethodType (typeInfo, methodInfo);
			ParameterInfo [] parameters = methodInfo.GetParameters ();

			StringBuilder nm = new StringBuilder ("?", 30);

			if (methodType == MethodType.NativeCtor)
				nm.Append ("?0");
            else if (methodType == MethodType.NativeDtor)
                nm.Append("?1");
            else
            {
                backReferences.Add(methodName);
                nm.Append(methodName).Append('@');
            }

            backReferences.Add(className);
            nm.Append(className);
            nm.Append("@");
            
            // FIXME: This has to include not only the name of the immediate containing class,
			//  but also all names of containing classes and namespaces up the hierarchy.
            if (type.Namespaces != null)
            {
                foreach (var ns in type.Namespaces.Reverse())
                {
                    backReferences.Add(ns);
                    nm.Append(ns).Append("@");
                }
            }

		    nm.Append ("@");

			// function modifiers are a matrix of consecutive uppercase letters
			// depending on access type and virtual (far)/static (far)/far modifiers

			// first, access type
			char funcModifier = 'Q'; // (public)
			if (IsProtected (methodInfo))
				funcModifier = 'I';
			else if (IsPrivate (methodInfo)) // (probably don't need this)
				funcModifier = 'A';

			// now, offset based on other modifiers
			if (IsStatic (methodInfo))
				funcModifier += (char)2;
			else if (IsVirtual (methodInfo))
				funcModifier += (char)4;

			nm.Append (funcModifier);

			// FIXME: deal with other storage classes for "this" i.e. the "volatile" in -> int foo () volatile;
			if (!IsStatic (methodInfo)) {
				if (IsConst (methodInfo))
					nm.Append ('B');
				else
					nm.Append ('A');
			}

			switch (GetCallingConvention (methodInfo)) {
			case CallingConvention.Cdecl:
				nm.Append ('A');
				break;
			case CallingConvention.ThisCall:
				nm.Append ('E');
				break;
			case CallingConvention.StdCall:
				nm.Append ('G');
				break;
			case CallingConvention.FastCall:
				nm.Append ('I');
				break;
			}

			// FIXME: handle const, volatile modifiers on return type
			// FIXME: the manual says this is only omitted for simple types.. are we doing the right thing here?

            if (methodType == MethodType.NativeCtor || methodType == MethodType.NativeDtor)
            {
                nm.Append('@');
            }
            else
            {
                CppType returnType = GetMangleType(methodInfo.ReturnTypeCustomAttributes, methodInfo.ReturnType);
				// TODO: Should this actually be done in CppType.ToManagedType
				// I wasn't sure how it would affect Itanium mangled names
                bool hadPointer = false;
                if (methodInfo.ReturnTypeCustomAttributes.IsDefined(typeof(ByValAttribute), false))
                {
                    hadPointer = returnType.Modifiers.Remove(CppModifiers.Pointer);

                    if (returnType.ElementType == CppTypes.Class ||
                        returnType.ElementType == CppTypes.Struct ||
                        returnType.ElementType == CppTypes.Union)
                        nm.Append("?A");
                }

                nm.Append(GetTypeCode(returnType, backReferences));
                if (hadPointer)
                    returnType.Modifiers.Add(CppModifiers.Pointer);
            }

            
		    int argStart = (IsStatic (methodInfo)? 0 : 1);
            if (parameters.Length == argStart)
            { // no args (other than C++ "this" object)
                nm.Append("XZ");
                return nm.ToString();
            }

            var argumentBackReferences = new BackReferenceList();
            for (int i = argStart; i < parameters.Length; i++)
            {
                var typeCode = GetTypeCode(GetMangleType(parameters[i], parameters[i].ParameterType), backReferences);
                nm.Append(argumentBackReferences.Add(typeCode));
            }

		    nm.Append ("@Z");
			return nm.ToString ();
		}

        public virtual string GetTypeCode(CppType mangleType, BackReferenceList backReferences)
		{
			CppTypes element = mangleType.ElementType;
			IEnumerable<CppModifiers> modifiers = mangleType.Modifiers;

			StringBuilder code = new StringBuilder ();

			var ptr = For.AnyInputIn (CppModifiers.Pointer);
			var ptrRefOrArray = For.AnyInputIn (CppModifiers.Pointer, CppModifiers.Reference, CppModifiers.Array);

			var modifierCode = modifiers.Reverse ().Transform (

				Choose.TopOne (
					For.AllInputsIn (CppModifiers.Const, CppModifiers.Volatile).InAnyOrder ().After (ptrRefOrArray).Emit ('D'),
			                For.AnyInputIn (CppModifiers.Const).After (ptrRefOrArray).Emit ('B'),
					For.AnyInputIn (CppModifiers.Volatile).After (ptrRefOrArray).Emit ('C'),
			                For.AnyInput<CppModifiers> ().After (ptrRefOrArray).Emit ('A')
			        ),

				For.AnyInputIn (CppModifiers.Array).Emit ('Q'),
				For.AnyInputIn (CppModifiers.Reference).Emit ('A'),

				Choose.TopOne (
			                ptr.After ().AllInputsIn (CppModifiers.Const, CppModifiers.Volatile).InAnyOrder ().Emit ('S'),
					ptr.After ().AnyInputIn (CppModifiers.Const).Emit ('Q'),
			                ptr.After ().AnyInputIn (CppModifiers.Volatile).Emit ('R'),
			                ptr.Emit ('P')
                                ),

			        ptrRefOrArray.AtEnd ().Emit ('A')
			);
			code.Append (modifierCode.ToArray ());

            // Type codes taken from http://www.kegel.com/mangle.html#operators
		    switch (element) {
			case CppTypes.Void:
				code.Append ('X');
				break;
			case CppTypes.Int:
				code.Append (modifiers.Transform (Choose.TopOne(
					For.AllInputsIn (CppModifiers.Unsigned, CppModifiers.Short).InAnyOrder ().Emit ('G'),
                    For.AllInputsIn (CppModifiers.Short).InAnyOrder().Emit('F'),
                    For.AllInputsIn (CppModifiers.Unsigned, CppModifiers.Long).InAnyOrder().Emit('K'),
                    For.AllInputsIn (CppModifiers.Long).InAnyOrder().Emit('J'),
                    For.AllInputsIn (CppModifiers.Unsigned).InAnyOrder().Emit('I')
				)).DefaultIfEmpty ('H').ToArray ());
                break;
            case CppTypes.Float:
                code.Append('M');
                break;
            case CppTypes.Double:
                code.Append(modifiers.Transform(
                    For.AllInputsIn (CppModifiers.Long).InAnyOrder().Emit('O')
                ).DefaultIfEmpty ('N').ToArray());
                break;
            case CppTypes.Char:
                code.Append (modifiers.Transform (Choose.TopOne(
					For.AllInputsIn (CppModifiers.Unsigned).InAnyOrder ().Emit ('E'),
					For.AllInputsIn (CppModifiers.Signed).InAnyOrder ().Emit ('C')
				)).DefaultIfEmpty ('D').ToArray());
                break;
			case CppTypes.Class:
				code.Append ('V');
		        code.Append(GetTypeNameWithBackReferences(mangleType, backReferences));
		        code.Append ("@");
				break;
			case CppTypes.Struct:
				code.Append ('U');
                code.Append(GetTypeNameWithBackReferences(mangleType, backReferences));
                code.Append ("@");
				break;
			case CppTypes.Union:
				code.Append ('T');
                code.Append(GetTypeNameWithBackReferences(mangleType, backReferences));
                code.Append ("@");
				break;
			case CppTypes.Enum:
				code.Append ("W4");
                code.Append(GetTypeNameWithBackReferences(mangleType, backReferences));
                code.Append ("@");
				break;
            case CppTypes.Bool:
                code.Append ("_N");
                break;
			}

			return code.ToString ();
		}

        private static string GetTypeNameWithBackReferences(CppType mangleType, BackReferenceList backReferences)
	    {
	        var sb = new StringBuilder();
	        sb.Append(GetTypeCodeOrBackReference(mangleType.ElementTypeName, backReferences));
	        if (mangleType.Namespaces != null)
	        {
	            foreach (var ns in mangleType.Namespaces.Reverse())
	            {
	                sb.Append(GetTypeCodeOrBackReference(ns, backReferences));
	            }
	        }
	        return sb.ToString();
	    }

	    private static string GetTypeCodeOrBackReference(string elementTypeName, BackReferenceList backReferences)
	    {
	        if (backReferences.Contains(elementTypeName)) 
                return backReferences[elementTypeName];

	        return elementTypeName + "@";
	    }

        protected override bool ReturnByHiddenArgument (CppTypeInfo typeInfo, MethodInfo method)
        {
            return IsByVal(method.ReturnTypeCustomAttributes);
        }

	    public class BackReferenceList
        {
            private readonly Dictionary<string, int> _backReferences = new Dictionary<string, int>();

            public string this[string key] 
            { 
                get
                {
                    return _backReferences[key].ToString();
                }
            }

            public bool Contains(string value)
            {
                return _backReferences.ContainsKey(value);
            }

            public string Add(string value)
            {
                if (value.Length == 1)
                    return value;

                if (_backReferences.ContainsKey(value))
                    return _backReferences[value].ToString();

                _backReferences.Add(value, _backReferences.Count);

                return value;
            }
        }
	}
}

