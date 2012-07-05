using System;
using System.Collections.Generic;
using System.Text;
using Tests;

namespace Tester
{
    class Program
    {
        static void Main (string[] args)
        {
			var fieldTest = new FieldTests();
			fieldTest.TestReadCppObject();

			var inheritTest = new InheritanceTests();
			inheritTest.TestManagedOverride1();
			inheritTest.TestManagedOverride2();

			inheritTest.TestMultipleBases();
			inheritTest.TestMultipleVirtualBases();
			inheritTest.TestNonVirtualCallOnVirtualBaseClass();
			inheritTest.TestRoundtripManagedOverride();
			inheritTest.TestVirtualCall();
			inheritTest.TestVirtualCallOnBaseClass();
			inheritTest.TestVirtualCallOnVirtualBaseClass();

			var marshalingTest = new MarshalingTests();
			marshalingTest.TestByRefReturn();
			marshalingTest.TestClassArg();
			marshalingTest.TestClassArgByval();

			marshalingTest.TestClassArgByvalNull();

			marshalingTest.TestClassArgNull();
			marshalingTest.TestClassReturn();
			marshalingTest.TestPrimitiveReturn();

			var templateTest = new TemplateTests();
			templateTest.TestDoubleTemplate();
			templateTest.TestFloatTemplate();
			templateTest.TestIntTemplate();
			templateTest.TestShortTemplate();

            var tests = new ManglingTests();
            tests.TestCompression();
			tests.TestNamespace();
			tests.TestNamespace2();
			tests.TestNamespace3();
			tests.TestNamespace4();
			tests.TestNamespaced2();
        }
    }
}
