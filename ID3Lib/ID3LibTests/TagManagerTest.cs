using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Id3Lib.Tests
{
    [TestClass]
    public class TagManagerTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Read221Compressed()
        {
            TagManager.Deserialize(Resources.GetResource("221-Compressed.tag"));
        }

        [TestMethod]
        public void Read230Compressed()
        {
            TagManager.Deserialize(Resources.GetResource("230-Compressed.tag"));
        }

        [TestMethod]
        public void Read230Picture()
        {
            TagManager.Deserialize(Resources.GetResource("230-Picture.tag"));
        }

        [TestMethod]
        public void Read230Syncedlyrics()
        {
            TagManager.Deserialize(Resources.GetResource("230-SyncedLyrics.tag"));
        }

        [TestMethod]
        public void Read230Unicode()
        {
            TagManager.Deserialize(Resources.GetResource("230-Unicode.tag"));
        }

        [TestMethod]
        public void Read230BarkMoon()
        {
            TagManager.Deserialize(Resources.GetResource("230-BarkMoon.tag"));
        }
    }
}
