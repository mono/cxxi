using System;
using System.Collections.Generic;
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
            var a = new AdderTemplateOfDouble();
            Assert.AreEqual(0.0d, a.Number, "#1");

            a.Add(5.33d);
            Assert.AreEqual(5.33d, a.Number, "#2");
        }

        [Test]
        public void TestFloatTemplate()
        {
            var a = new AdderTemplateOfFloat(2.5f);
            Assert.AreEqual(2.5f, a.Number, "#1");

            a.Add(2.5f);
            Assert.AreEqual(5.0f, a.Number, "#2");
        }

        [Test]
        public void TestIntTemplate()
        {
            var a = new AdderTemplateOfInt(10);
            Assert.AreEqual(10, a.Number, "#1");

            a.Add(-5);
            Assert.AreEqual(5, a.Number, "#2");
        }

        [Test]
        public void TestShortTemplate()
        {
            var a = new AdderTemplateOfShortInt();
            Assert.AreEqual(0, a.Number, "#1");

            a.Add(123);
            Assert.AreEqual(123, a.Number, "#2");
        }
    }
}
