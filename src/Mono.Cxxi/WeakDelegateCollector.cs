using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mono.Cxxi
{
    public static class WeakDelegateCollector
    {
        public const int CollectionIntervalMs = 5000;

        private static readonly Dictionary<Delegate, Delegate> StaticDelegates =
            new Dictionary<Delegate, Delegate> ();
        private static readonly Dictionary<IWeakDelegate, IWeakDelegate> InstanceDelegates =
            new Dictionary<IWeakDelegate, IWeakDelegate> ();

        public static T GetStaticDelegate<T> (T @delegate)
            where T : class
        {
            Delegate del = (Delegate)(object)@delegate;
            System.Diagnostics.Debug.Assert (del.Target == null);

            // Find an existing static delegate
            Delegate ret = null;
            lock (StaticDelegates) {
                if (!StaticDelegates.TryGetValue (del, out ret)) {
                    ret = del;
                    StaticDelegates.Add (del, del);
                }
            }

            return ret as T;
        }

        public static void RegisterWeakDelegate (IWeakDelegate weak)
        {
            // If this is not an instance delegate we haven nothing to register
            if (weak.Reference.Target == null)
                return;

            bool startThread = false;

            // Add a reference to our instance delegate
            lock (InstanceDelegates) {
                if (!InstanceDelegates.ContainsKey (weak)) {
                    InstanceDelegates.Add (weak, weak);
                    startThread = (InstanceDelegates.Count == 1 && CollectionThread == null);
                    if (startThread)
                        CollectionThread = new Thread (RunCollectionThread);
                }
            }

            // If we need to start our garbage collection thread then start it up
            if (startThread) {
                CollectionThread.IsBackground = true;
                CollectionThread.Start ();
            }
        }

        public static void UnregisterWeakDelegate (IWeakDelegate weak)
        {
            lock (InstanceDelegates) {
                InstanceDelegates.Remove (weak);
            }
        }

        private static Thread CollectionThread = null;
        private static void RunCollectionThread ()
        {
            var watch = new Stopwatch ();
            watch.Start ();

            while (true) {
                List<IWeakDelegate> list;

                lock (InstanceDelegates) {
                    // If we have no more delegates then check to see if we should exit
                    if (InstanceDelegates.Count == 0) {
                        // Give us an extra 5 seconds to before exiting
                        Monitor.Wait (InstanceDelegates, CollectionIntervalMs);
                        if (InstanceDelegates.Count == 0) {
                            CollectionThread = null;
                            return;
                        }
                    }

                    // Wait until time for the next garbage collection interval
                    int remaining = (int)(CollectionIntervalMs - watch.ElapsedMilliseconds);
                    if (remaining > 0) {
                        Monitor.Wait (InstanceDelegates, remaining);
                        continue;
                    }

                    // Get a copy of the list of weak delegates to use outside the lock
                    list = new List<IWeakDelegate> (InstanceDelegates.Values);
                }

                // Check each delegate tos see if it needs to be collected
                foreach (var weak in list) {
                    if (!weak.CheckDelegate ())
                        Trace.TraceInformation ("Garbage collected delegate.");
                }

                // Restart our stopwatch after each check
                watch.Reset();
                watch.Start();
            }
        }

    }

}
