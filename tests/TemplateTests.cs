using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TemplateTests
    {
        [Test]
        public void TestDoubleTemplate()
        {
            var a = new AdderDouble();
            Assert.AreEqual(0.0d, t.Number, "#1");

            a.Add(5.33d);
            Assert.AreEqual(5.33d, t.Number, "#2");
        }

        [Test]
        public void TestDoubleTemplate()
        {
            var a = new AdderFloat(2.5f);
            Assert.AreEqual(2.5f, t.Number, "#1");

            a.Add(2.5f);
            Assert.AreEqual(5.0f, t.Number, "#2");
        }

        [Test]
        public void TestIntTemplate()
        {
            var a = new AdderInt(10);
            Assert.AreEqual(10, t.Number, "#1");

            a.Add(-5);
            Assert.AreEqual(5, t.Number, "#2");
        }

        [Test]
        public void TestShortTemplate()
        {
            var a = new AdderShort();
            Assert.AreEqual(0, t.Number, "#1");

            a.Add(123);
            Assert.AreEqual(123, t.Number, "#2");
        }
    }
}
