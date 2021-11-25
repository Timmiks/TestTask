using NUnit.Framework;
using System;

namespace MonitorUnitTests.Utility
{
    public static class CustomAssert
    {
        public static void AreEqual(DateTime expected, DateTime actual, TimeSpan delta)
        {
            Assert.IsTrue(actual.Subtract(expected) <= delta, $"Expected: time between {expected} and {expected.Add(delta)}\r\n But was: {actual}");
        }
    }
}
