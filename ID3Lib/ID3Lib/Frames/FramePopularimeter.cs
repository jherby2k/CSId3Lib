// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manage popularimeter frames.
    /// </summary>
    /// <remarks>
    ///   The purpose of this frame is to specify how good an audio file is.
    /// </remarks>
    [PublicAPI]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Popularimeter")]
    [Frame("POPM")]
    public class FramePopularimeter : FrameBase, IFrameDescription
    {
        [NotNull] byte[] _counter = { 0 };

        /// <summary>
        /// The rating is 1-255 where 1 is worst and 255 is best. 0 is unknown.
        /// </summary>
        public byte Rating { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get the number of times the song has been played
        /// </summary>
        public ulong Counter
        {
            get => Memory.ToInt64(_counter);
            set => _counter = Memory.GetBytes(value);
        }

        /// <summary>
        /// Create a Play Counter frame.
        /// </summary>
        /// <param name="frameId">ID3v2 POPM frame</param>
        public FramePopularimeter([NotNull] string frameId)
            : base(frameId)
        {
        }

        /// <summary>
        /// Parse the binary POPM frame
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            var index = 0;
            Description = TextBuilder.ReadASCII(frame, ref index);
            Rating = frame[index++];
            _counter = Memory.Extract(frame, index, frame.Length - index);
        }

        /// <summary>
        /// Create a binary POPM frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                writer.Write(TextBuilder.WriteASCII(Description));
                writer.Write(Rating);
                writer.Write(_counter);
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Unique Tag Identifer description 
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public override string ToString()
        {
            return string.Empty;
        }
    }
}
