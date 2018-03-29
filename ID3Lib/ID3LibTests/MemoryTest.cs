// Copyright(C) 2002-2009 Hugo Rumayor Montemayor, All rights reserved.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Id3Lib.Tests
{
    [TestClass]
    public class MemoryTest
    {
        [TestMethod]
        [Description("Test conversion from/to unsigned long to a byte array")]
        public void GetBytes()
        {
            byte[] bitTests = { 0, 2, 8, 16, 18, 24, 32, 40, 55, 60 };
            foreach (var bits in bitTests)
            {
                var original = (ulong) Math.Pow(2, bits);
                var bytes = Memory.GetBytes(original);

                //int length = (bits / 8) + 1;
                //Assert.AreEqual(length <= 4 ? 4 : length, bytes.Length);
                Assert.AreEqual(8, bytes.Length);
                Assert.AreEqual(original, Memory.ToInt64(bytes));
            }
        }
    }
}
