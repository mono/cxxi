using System;
using NUnit.Framework;

namespace Tests {

	[TestFixture]
	public class FieldTests {

		[Test]
		public void TestReadCppObject ()
		{
			var hf1 = new HasField (1, null);
			var hf2 = new HasField (2, hf1);
			var hf3 = new HasField (3, hf2);

			Assert.IsNull (hf1.other, "#1");
			Assert.AreEqual (1, hf1.number, "#2");

            Assert.That (hf2.other == hf1, "#3");
            Assert.AreEqual (hf2.other, hf1, "#4");
			Assert.AreEqual (1, hf2.other.number, "#5");

			Assert.That (hf3.other.other == hf1, "#6");
			Assert.AreEqual (hf3.other.other, hf1, "#7");
			Assert.AreEqual (1, hf3.other.other.number, "#8");
		}
	}
}

