using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Id3Lib.Frames;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Id3Lib.Tests
{
    [TestClass]
    public class FrameHelperTest
    {
        [TestMethod]
        public void TagCompression()
        {
            TagModel frameModel = new TagModel();

            FrameHelper frameHelper = new FrameHelper(frameModel.Header);

            FrameText originalFrame = (FrameText)FrameFactory.Build("TALB");
            originalFrame.Text = "Hello World!!!";
            originalFrame.Compression = true;

            ushort flags;
            byte[] body = frameHelper.Make(originalFrame, out flags);

            FrameText resultFrame = (FrameText)frameHelper.Build("TALB", flags, body);

            Assert.AreEqual(resultFrame.Text, originalFrame.Text);
        }

    }
}
