// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manage general encapsulated objects.
    /// </summary>
    /// <remarks>
    /// The <b>FrameBinary</b> class handles GEOB ID3v2 frame types that can hold any type of file
    /// or binary data encapsulated.
    /// </remarks>
    [PublicAPI]
    [Frame("GEOB")]
    public class FrameBinary : FrameBase, IFrameDescription
    {
        string _fileName;

        /// <summary>
        /// Type of text encoding
        /// </summary>
        public TextCode TextEncoding { get; set; }

        /// <summary>
        /// Text MIME type
        /// </summary>
        [NotNull]
        public string Mime { get; set; }

        /// <summary>
        /// Frame description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Binary representation of the object
        /// </summary>
        [NotNull]
        public byte[] ObjectData { get; set; }

        /// <summary>
        /// Create a FrameGEOB frame.
        /// </summary>
        /// <param name="frameId">ID3v2 GEOB frame</param>
        public FrameBinary([NotNull] string frameId)
            : base(frameId)
        {
            TextEncoding = TextCode.Ascii;
        }

        /// <summary>
        /// Parse the binary GEOB frame
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");

            var index = 0;
            TextEncoding = (TextCode) frame[index++];
            Mime = TextBuilder.ReadASCII(frame, ref index);
            _fileName = TextBuilder.ReadText(frame, ref index, TextEncoding);
            Description = TextBuilder.ReadText(frame, ref index, TextEncoding);
            ObjectData = Memory.Extract(frame, index, frame.Length - index);
        }

        /// <summary>
        /// Create a binary GEOB frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                writer.Write((byte) TextEncoding);
                writer.Write(TextBuilder.WriteASCII(Mime));
                writer.Write(TextBuilder.WriteText(_fileName, TextEncoding));
                writer.Write(TextBuilder.WriteText(Description, TextEncoding));
                writer.Write(ObjectData);
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// GEOB frame description 
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public override string ToString()
        {
            return Description;
        }
    }
}
