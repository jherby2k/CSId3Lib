// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manages Text frames
    /// </summary>
    /// <remarks>
    /// The <b>FrameText</b> class handles the frames of text information these 
    /// are often the most important frames, containing information like artist,
    /// album and more. There may only be one text information frame of its kind in an tag.
    /// 
    /// Text encoding        $xx
    /// Information          text string(s) according to encoding
    /// </remarks>
    [PublicAPI]
    [Frame("T")]
    public class FrameText : FrameBase
    {
        /// <summary>
        /// Get or set the type of text encoding, the frame is using.
        /// </summary>
        public TextCode TextCode { get; set; } = TextCode.Ascii;

        /// <summary>
        /// Get or Set the text of the frame
        /// </summary>
        [NotNull]
        public string Text { get; set; }

        /// <summary>
        /// Create a FrameText frame.
        /// </summary>
        /// <param name="frameId">ID3v2 type of text frame</param>
        public FrameText([NotNull] string frameId)
            : base(frameId)
        {
        }

        /// <summary>
        /// Parse the text binary frame.
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            var index = 0;
            TextCode = (TextCode) frame[index++];
            Text = TextBuilder.ReadTextEnd(frame, index, TextCode);
        }

        /// <summary>
        /// Create a text binary frame.
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                writer.Write((byte) TextCode);
                writer.Write(TextBuilder.WriteTextEnd(Text, TextCode));
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Default Frame description.
        /// </summary>
        /// <returns>Text of the frame</returns>
        [NotNull]
        public override string ToString()
        {
            return Text;
        }
    }
}
