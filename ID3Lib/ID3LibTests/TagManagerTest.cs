using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
            var model = TagManager.Deserialize(Resources.GetResource("221-Compressed.tag"));
        }

        [TestMethod]
        public void Read230Compressed()
        {
            var model = TagManager.Deserialize(Resources.GetResource("230-Compressed.tag"));
        }

        [TestMethod]
        public void Read230Picture()
        {
            var model = TagManager.Deserialize(Resources.GetResource("230-Picture.tag"));
        }


        [TestMethod]
        public void Read230Syncedlyrics()
        {
            var model = TagManager.Deserialize(Resources.GetResource("230-SyncedLyrics.tag"));
        }


        [TestMethod]
        public void Read230Unicode()
        {
            var model = TagManager.Deserialize(Resources.GetResource("230-Unicode.tag"));
        }


        [TestMethod]
        public void Read230BarkMoon()
        {
            var model = TagManager.Deserialize(Resources.GetResource("230-BarkMoon.tag"));
        }

       
    }
}
