using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Mono.Cxxi
{
    public interface IWeakDelegate
    {
        Type Type { get; }
        MethodInfo Method { get; }
        WeakReference Reference { get; }
        bool CheckDelegate ();
    }

    public class WeakDelegate<T> : IWeakDelegate
        where T : class
    {
        public Action<T> Callback { get; set; }

        public WeakDelegate (T @delegate)
            : this (@delegate, null)
        {
        }

        public WeakDelegate (T @delegate, Action<T> callback)
        {
            this.Callback = callback;
            var del = (Delegate)(object)@delegate;
            this.Type = del.GetType ();
            this.Method = del.Method;
            if (del.Target != null) {
                this.Reference = new WeakReference (del.Target);
                this.WrapperDelegate = this.CreateDelegate ();
            }

            WeakDelegateCollector.RegisterWeakDelegate (this);
            if (del.Target == null)
                this.WrapperDelegate = WeakDelegateCollector.GetStaticDelegate (@delegate);
        }

        public static implicit operator T (WeakDelegate<T> weak)
        {
            return weak.WrapperDelegate;
        }

        protected static readonly MethodInfo CheckDelegateMethod = typeof (WeakDelegate<T>).GetMethod ("CheckDelegate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        protected static readonly MethodInfo GetReferenceMethod = typeof (WeakDelegate<T>).GetProperty ("Reference").GetGetMethod ();
        protected static readonly MethodInfo WeakReferenceGetTargetMethod = typeof (WeakReference).GetProperty ("Target").GetGetMethod ();
        protected T CreateDelegate ()
        {
            var parameters = new List<Type> ();
            parameters.Add (this.GetType ());
            parameters.AddRange (this.Method.GetParameters ().Select (p => p.ParameterType));
            var method = new DynamicMethod ("WeakDelegateDynamic", this.Method.ReturnType,
                parameters.ToArray (), this.GetType (), true);
            var hasReturnType = (this.Method.ReturnType != typeof (void));

            if (this.Method.IsGenericMethod)
                method.MakeGenericMethod (this.Method.GetGenericArguments ());

            var il = method.GetILGenerator ();

            // IL Code should be equivalent of...
            /*
             * var target = this.Reference.Target;
             * if (!CheckDelegate())
             *   return;
             * 
             * return this.Method(args);
             * 
             */

            // Create our labels
            var returnLabel = il.DefineLabel ();
            var callLabel = il.DefineLabel ();

            // Define our local variables
            il.DeclareLocal (typeof (object));
            il.DeclareLocal (hasReturnType ? this.Method.ReturnType : typeof (int));
            il.DeclareLocal (typeof (bool));

            // Load the target value on to the stack to keep it alive
            il.Emit (OpCodes.Ldarg_0);
            il.Emit (OpCodes.Call, GetReferenceMethod);
            il.Emit (OpCodes.Callvirt, WeakReferenceGetTargetMethod);
            il.Emit (OpCodes.Stloc_0);

            // Call CheckDelegate
            il.Emit (OpCodes.Ldarg_0);
            il.Emit (OpCodes.Call, CheckDelegateMethod);
            il.Emit (OpCodes.Stloc_2);

            // If it did not return true then return immediately
            il.Emit (OpCodes.Ldloc_2);
            il.Emit (OpCodes.Brtrue_S, callLabel);
            il.Emit (OpCodes.Ldc_I4_0);
            il.Emit (OpCodes.Stloc_1);
            il.Emit (OpCodes.Br_S, returnLabel);

            // Load our target and all of our arguments
            il.MarkLabel (callLabel);
            il.Emit (OpCodes.Ldloc_0);
            for (int i = 0; i < this.Method.GetParameters ().Length; i++)
                il.Emit (OpCodes.Ldarg, i + 1);

            // Call our method and return
            il.Emit (this.Method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, this.Method);
            if (hasReturnType)
                il.Emit (OpCodes.Stloc_1);
            il.Emit (OpCodes.Br_S, returnLabel);
            il.MarkLabel (returnLabel);
            if (hasReturnType)
                il.Emit (OpCodes.Ldloc_1);
            il.Emit (OpCodes.Ret);

            return method.CreateDelegate (this.Type, this) as T;
        }

        public bool CheckDelegate ()
        {
            if (this.Reference.Target == null) {
                Action<T> callback = null;
                lock (this) {
                    callback = this.Callback;
                    this.Callback = null;
                }

                if (callback != null)
                    callback (this.WrapperDelegate);

                WeakDelegateCollector.UnregisterWeakDelegate (this);
                return false;
            }

            return true;
        }

        public T WrapperDelegate
        {
            get;
            private set;
        }

        public Type Type
        {
            get;
            private set;
        }

        public WeakReference Reference
        {
            get;
            private set;
        }

        public MethodInfo Method
        {
            get;
            private set;
        }
    }

    public class WeakEventHandler : WeakDelegate<EventHandler>
    {
        public WeakEventHandler (EventHandler @delegate)
            : base (@delegate)
        {
        }

        public WeakEventHandler (EventHandler @delegate, Action<EventHandler> callback)
            : base (@delegate, callback)
        {
        }
    }

    public class WeakEventHandler<T> : WeakDelegate<EventHandler<T>>
        where T : EventArgs
    {
        public WeakEventHandler (EventHandler<T> @delegate)
            : base (@delegate)
        {
        }

        public WeakEventHandler (EventHandler<T> @delegate, Action<EventHandler<T>> callback)
            : base (@delegate, callback)
        {
        }
    }
}
