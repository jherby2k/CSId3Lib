// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manages User defined URL WXXX Frames
    /// </summary>
    /// <remarks>
    /// Text encoding     $xx
    /// Description       text string according to encoding, $00 (00)
    /// URL               text string
    /// </remarks>
    [PublicAPI]
    [Frame("WXXX")]
    public class FrameUrlUserDef : FrameBase, IFrameDescription
    {
        /// <summary>
        /// Type of text encoding the frame is using
        /// </summary>
        public TextCode TextCode { get; set; } = TextCode.Ascii;

        /// <summary>
        /// Description of the frame contents
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The URL page location
        /// </summary>
        [NotNull]
        public string URL { get; set; }

        /// <summary>
        /// Create a URL frame
        /// </summary>
        /// <param name="frameId">Type of URL frame</param>
        public FrameUrlUserDef([NotNull] string frameId)
            : base(frameId)
        {
        }

        /// <summary>
        /// Parse the binary frame
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            //TODO: Handle this invalid tag
            if (frame.Length < 1)
                return;

            var index = 0;
            TextCode = (TextCode)frame[index++];
            Description = TextBuilder.ReadText(frame, ref index, TextCode);
            URL = TextBuilder.ReadTextEnd(frame, index, TextCode);
        }

        /// <summary>
        /// Create a binary frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                writer.Write((byte) TextCode);
                writer.Write(TextBuilder.WriteText(Description, TextCode));
                writer.Write(TextBuilder.WriteTextEnd(URL, TextCode));
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Default frame description
        /// </summary>
        /// <returns>URL text</returns>
        [NotNull]
        public override string ToString()
        {
            return URL;
        }
    }
}
