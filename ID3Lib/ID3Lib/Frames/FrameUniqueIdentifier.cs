// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manage unique identifier frames.
    /// </summary>
    /// <remarks>
    ///   This frame's purpose is to be able to identify the audio file in a
    ///   database, that may provide more information relevant to the content.
    /// </remarks>
    [PublicAPI]
    [Frame("UFID")]
    public class FrameUniqueIdentifier : FrameBase, IFrameDescription
    {
        [NotNull] byte[] _identifer;

        /// <summary>
        /// Frame description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Binary representation of the object
        /// </summary>
        [NotNull]
        public byte[] Identifier
        {
            get => _identifer;
            set
            {
                if (value.Length > 64)
                    throw new ArgumentOutOfRangeException("value", "The identifier can't be more than 64 bytes");
                _identifer = value;
            }
        }

        /// <summary>
        /// Create a FrameGEOB frame.
        /// </summary>
        /// <param name="frameId">ID3v2 UFID frame</param>
        public FrameUniqueIdentifier([NotNull] string frameId)
            : base(frameId)
        {
        }

        /// <summary>
        /// Parse the binary UFID frame
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            var index = 0;
            Description = TextBuilder.ReadASCII(frame, ref index);
            _identifer = Memory.Extract(frame, index, frame.Length - index);
        }

        /// <summary>
        /// Create a binary UFID frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                writer.Write(TextBuilder.WriteASCII(Description));
                writer.Write(_identifer);
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Unique Tag Identifier description 
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public override string ToString()
        {
            return Description;
        }
    }
}
