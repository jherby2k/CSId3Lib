// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manages unsynchronised lyrics and comment frames.
    /// </summary>
    /// <remarks>
    /// Text encoding        $xx
    /// Language             $xx xx xx
    /// Content descriptor   text string according to encoding, $00 (00)
    /// Lyrics/text          full text string according to encoding
    /// </remarks>
    [PublicAPI]
    [Frame("USLT"), Frame("COMM")]
    public class FrameFullText : FrameBase, IFrameDescription
    {
        /// <summary>
        /// Get or set the type of text encoding the frame is using.
        /// </summary>
        public TextCode TextCode { get; set; } = TextCode.Ascii;

        /// <summary>
        /// Get or set the description of the frame contents.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get or set the main text data.
        /// </summary>
        [NotNull]
        public string Text { get; set; }

        /// <summary>
        /// Get or set the Language the main text uses.
        /// </summary>
        [NotNull]
        public string Language { get; set; } = "eng";

        /// <summary>
        /// Create a FrameLCText frame.
        /// </summary>
        /// <param name="frameId">ID3v2 type of text frame</param>
        public FrameFullText([NotNull] string frameId)
            : base(frameId)
        {
        }

        /// <summary>
        /// Parse binary data unsynchronised lyrics/comment frame.
        /// </summary>
        /// <param name="frame">binary frame data</param>
        public override void Parse(byte[] frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");

            var index = 0;
            TextCode = (TextCode) frame[index];
            index++;

            //TODO: Invalid tag, may be legacy.
            if (frame.Length - index < 3)
                return;

            Language = Encoding.UTF8.GetString(frame, index, 3);
            index += 3; // Three language bytes

            if (frame.Length - index < 1)
                return;

            Description = TextBuilder.ReadText(frame, ref index, TextCode);
            Text = TextBuilder.ReadTextEnd(frame, index, TextCode);
        }

        /// <summary>
        /// Create binary data from unsynchronised lyrics/comment frame 
        /// </summary>
        /// <returns>binary frame data</returns>
        public override byte[] Make()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                writer.Write((byte) TextCode);
                //TODO: Validate language field
                var language = TextBuilder.WriteASCII(Language);
                if (language.Length != 3)
                    writer.Write(new[] {(byte) 'e', (byte) 'n', (byte) 'g'});
                else
                    writer.Write(language, 0, 3);
                writer.Write(TextBuilder.WriteText(Description, TextCode));
                writer.Write(TextBuilder.WriteTextEnd(Text, TextCode));
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Default frame description.
        /// </summary>
        /// <returns>unsynchronised lyrics/comment text</returns>
        [NotNull]
        public override string ToString()
        {
            return Text;
        }
    }
}
