using System;
using NUnit.Framework;

namespace Tests {

	[TestFixture]
	public class ManglingTests {

		[Test]
		public void TestCompression ()
		{
            float f1 = 0.0f, f2 = 0.0f;
            int i1 = 0, i2 = 0;
            double d1 = 0.0d, d2 = 0.0d;
			Compression.Test1 (null, "foo", null, "bar");

            var r1 = Compression.Test2(null, ref f1, ref i1, null, ref f2, ref i2, ref d1, ref d2, 5);
            Assert.AreEqual(5, r1, "#2");
		}

		[Test]
		public void TestNamespaced ()
		{
            Compression c = null;
            Tests.Ns1.Namespaced n = null;
            float f1 = 0.0f, f2 = 0.0f;
			Ns1.Namespaced.Test1 ();
			Ns1.Namespaced.Test2 (null);

            var r3 = Ns1.Namespaced.Test3(c, c, ref f1, ref f2, 22);
            Assert.AreEqual(22, r3, "#3");

            var r4 = Ns1.Namespaced.Test4(n, n, c, c, ref f1, ref f2, 44);
            Assert.AreEqual(44, r4, "#4");

            var r5 = Ns1.Namespaced.Test5(n, n, c, c, 88);
            Assert.AreEqual(88, r5, "#5");
		}

		[Test]
		public void TestNamespaced2 ()
		{
			var cls = new Ns1.Ns2.Namespaced2 ();
			cls.Test1 ();
			cls.Test2 (null);
		}

        [Test]
        public void TestNamespace4 ()
	    {
	        var r3 = Ns1.Ns2.Namespaced2.Test3(null, null, null, null, 11);
	        Assert.AreEqual(11, r3, "#3");
	    }

	    [Test]
        public void TestNamespace3 ()
	    {
	        float f1 = 0.0f, f2 = 0.0f;
	        var r6 = Ns1.Ns2.Namespaced2.Test6(null, null, ref f1, ref f2, 11111);
	        Assert.AreEqual(11111, r6, "#6");
	    }

        [Test]
        public void TestNamespace2 ()
	    {
	        var r5 = Ns1.Ns2.Namespaced2.Test5(null, null, null, null, 1111);
	        Assert.AreEqual(1111, r5, "#5");
	    }

	    [Test]
        public void TestNamespace ()
	    {
	        var r4 = Ns1.Ns2.Namespaced2.Test4(null, null, null, null, null, null, null, null, 111);
	        Assert.AreEqual(111, r4, "#4");
	    }
	}
}

