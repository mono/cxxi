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
            var tests = new ManglingTests();
            tests.TestCompression();
        }
    }
}
