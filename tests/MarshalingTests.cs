using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Tests {

	[TestFixture]
	public class MarshalingTests {

		[Test]
		public void TestClassReturn ()
		{
			// Section 3.1.4:
			// Classes with non-default copy ctors/destructors are returned using a hidden
			// argument
			var c = ClassWithCopyCtor.Return (42);
			Assert.AreEqual (42, c.GetX (), "#1");

			var c2 = ClassWithDtor.Return (43);
			Assert.AreEqual (43, c2.GetX (), "#2");

			// This class is returned normally
			var c3 = ClassWithoutCopyCtor.Return (44);
			Assert.AreEqual (44, c3.GetX (), "#3");
		}

        [Test]
        public void TestPrimitiveReturn()
        {
            var sc = Class.ReturnSChar(68);
            Assert.AreEqual((char)68, sc, "#1");

            var c = Class.ReturnChar(69);
            Assert.AreEqual((char)69, c, "#2");

            var uc = Class.ReturnUChar(70);
            Assert.AreEqual((char)70, uc, "#3");

            var s = Class.ReturnShort(71);
            Assert.AreEqual((short)71, s, "#4");

            var us = Class.ReturnUShort(72);
            Assert.AreEqual((ushort)72, us, "#5");

            var i = Class.ReturnInt(73);
            Assert.AreEqual((int)73, i, "#6");

            var ui = Class.ReturnUInt(74);
            Assert.AreEqual((uint)74, ui, "#7");

            var l = Class.ReturnLong(75);
            Assert.AreEqual((int)75, l, "#8");

            var ul = Class.ReturnULong(76);
            Assert.AreEqual((uint)76, ul, "#9");

            var f = Class.ReturnFloat(77);
            Assert.AreEqual((float)77, f, "#10");

            var d = Class.ReturnDouble(78);
            Assert.AreEqual((double)78, d, "#11");

            var ld = Class.ReturnDouble(79);
            Assert.AreEqual((double)79, ld, "#12");

            var bt = Class.ReturnBool(80);
            Assert.AreEqual(true, bt, "#13");

            var bf = Class.ReturnBool(0);
            Assert.AreEqual(false, bf, "#14");
        }

		// An object as ref argument
		[Test]
		public void TestClassArg ()
		{
			var c1 = new Class (4);
			var c2 = new Class (5);
	
			c1.CopyTo (c2);
			Assert.AreEqual (4, c2.GetX (), "#1");
		}

		// A null object as ref argument
		[Test]
		public void TestClassArgNull ()
		{
			var c1 = new Class (4);
			Assert.That (c1.IsNull (null), "#1");
		}

		// An object as byval argument
		[Test]
		public void TestClassArgByval ()
		{
			var c1 = new Class (4);
			var c2 = new Class (5);
	
			c1.CopyFromValue (c2);
			Assert.AreEqual (5, c1.GetX (), "#1");
		}
	
		// A null object as byval argument
		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void TestClassArgByvalNull ()
		{
			var c1 = new Class (4);

			try
			{
				// This statement should trigger an ArgumentException
				c1.CopyFromValue (null);
				Debug.Assert(false);
			}
			catch(ArgumentException)
			{

			}


		}

		[Test]
		public void TestByRefReturn ()
		{
			var c1 = new Class (7);
			Assert.AreEqual (7, c1.GetXRef ());
		}
	
	}

}