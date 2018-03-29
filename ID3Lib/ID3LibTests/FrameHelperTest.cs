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
            var frameHelper = new FrameHelper(new TagModel().Header);

            var originalFrame = (FrameText) FrameFactory.Build("TALB");
            originalFrame.Text = "Hello World!!!";
            originalFrame.Compression = true;

            var body = frameHelper.Make(originalFrame, out var flags);

            Assert.AreEqual(
                ((FrameText) frameHelper.Build("TALB", flags, body)).Text,
                originalFrame.Text);
        }

    }
}
